using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using MoonSharp.Interpreter;

namespace M8.Lua {
    using Modules;

    [AddComponentMenu("M8/Lua/Behaviour")]
    public class LuaBehaviour : MonoBehaviour {
        public const string luaPropertiesTable = "properties";

        public const string luaFuncAwake = "Awake";
        public const string luaFuncStart = "Start";
        public const string luaFuncOnEnable = "OnEnable";
        public const string luaFuncOnDisable = "OnDisable";
        public const string luaFuncOnDestroy = "OnDestroy";

        public enum LoadFrom {
            String,
            File,
            TextAsset,
        }

        [Serializable]
        public struct Variable {
            public enum Type {
                Boolean,
                Integer,
                Float,
                String,
                GameObject,
            }

            public string name;
            public Type type;

            public int iVal;
            public float fVal;
            public string sVal;
            public GameObject goVal;

            public void Reset() {
                iVal = 0;
                fVal = 0f;
                sVal = "";
                goVal = null;
            }

            public void AddToTable(Table t) {
                switch(type) {
                    case Type.Boolean:
                        t[name] = iVal > 0;
                        break;
                    case Type.Integer:
                        t[name] = iVal;
                        break;
                    case Type.Float:
                        t[name] = fVal;
                        break;
                    case Type.String:
                        t[name] = sVal;
                        break;
                    case Type.GameObject:
                        t[name] = goVal;
                        break;
                }
            }
        }

        public LoaderBase loaderOverride;

        [SerializeField]
        LoadFrom scriptFrom = LoadFrom.TextAsset;

        [SerializeField]
        string scriptPath;

        [SerializeField]
        TextAsset scriptText;

        [SerializeField]
        bool loadOnAwake = true;

        [SerializeField]
        Variable[] properties; //added to the lua environment before executing script

        private Script mScript;
        private DynValue mScriptResult;

        private string mScriptString; //code from runtime

        private object mScriptFuncEnable;
        private object mScriptFuncDisable;

        private bool mIsAwake;

        public Script script { get { return mScript; } }

        public DynValue result { get { return mScriptResult; } }

        public void SetSourceFromString(string s) {
            scriptFrom = LoadFrom.String;
            mScriptString = s;
        }

        public void SetSourceFromFile(string path) {
            scriptFrom = LoadFrom.File;
            scriptPath = path;
        }

        public void SetSourceFromTextAsset(TextAsset text) {
            scriptFrom = LoadFrom.TextAsset;
            scriptText = text;
        }

        /// <summary>
        /// Load the script based on source.  Make sure to call SetSource* beforehand.
        /// </summary>
        public void Load() {
            //reloading?
            if(mScript != null) {
                StopAllCoroutines();

                OnDestroy(); //ensure all ties are severed before reloading
            }

            //grab components for interfaces
            Component[] comps = GetComponentsInChildren<Component>(true);

            //initialize script
            CoreModules coreModules = GlobalSettings.instance ? GlobalSettings.instance.coreModules : GlobalSettings.coreModuleDefault;
            UnityCoreModules unityCoreModules = GlobalSettings.instance ? GlobalSettings.instance.unityCoreModules : GlobalSettings.unityCoreModuleDefault;

            mScript = new Script(coreModules);
            mScript.Globals.RegisterUnityCoreModules(unityCoreModules);

            if(loaderOverride)
                mScript.Options.ScriptLoader = loaderOverride;

            //add core component modules
            GameObjectModule.Register(mScript.Globals, gameObject);
            TransformModule.Register(mScript.Globals, transform);
            BehaviourModule.Register(mScript.Globals, this);

            //add properties
            if(properties != null) {
                Table propTable = new Table(mScript);
                mScript.Globals[luaPropertiesTable] = propTable;

                for(int i = 0; i < properties.Length; i++)
                    properties[i].AddToTable(propTable);
            }

            for(int i = 0; i < comps.Length; i++) {
                //debug stuff
                IDebugPrint p = comps[i] as IDebugPrint;
                if(p != null)
                    mScript.Options.DebugPrint += p.Print;

                //preload setup
                IInitialization m = comps[i] as IInitialization;
                if(m != null)
                    m.PreLoad(mScript);
            }

            //load
            try {
                switch(scriptFrom) {
                    case LoadFrom.File:
                        mScriptResult = mScript.DoFile(scriptPath);
                        break;
                    case LoadFrom.TextAsset:
                        mScriptResult = mScript.DoString(scriptText.text);
                        break;
                    case LoadFrom.String:
                        mScriptResult = mScript.DoString(mScriptString);
                        break;
                }
            }
            catch(InterpreterException ie) {
                Debug.LogError(ie.DecoratedMessage);
            }

            //post load
            for(int i = 0; i < comps.Length; i++) {
                //postload setup
                IInitialization m = comps[i] as IInitialization;
                if(m != null)
                    m.PostLoad(mScript);
            }

            mScriptFuncEnable = mScript.Globals[luaFuncOnEnable];
            mScriptFuncDisable = mScript.Globals[luaFuncOnDisable];

            //awake
            object awakeFunc = mScript.Globals[luaFuncAwake];
            if(awakeFunc != null) {
                try {
                    mScript.Call(awakeFunc);
                }
                catch(InterpreterException ie) {
                    Debug.LogError(ie.DecoratedMessage);
                }
            }

            //manually enable if we are instantiated before, and are active on scene
            if(mIsAwake && enabled && gameObject.activeInHierarchy)
                OnEnable();
        }

        void OnEnable() {
            if(mScriptFuncEnable != null) {
                try {
                    mScript.Call(mScriptFuncEnable);
                }
                catch(InterpreterException ie) {
                    Debug.LogError(ie.DecoratedMessage);
                }
            }
        }

        void OnDisable() {
            if(mScriptFuncDisable != null) {
                try {
                    mScript.Call(mScriptFuncDisable);
                }
                catch(InterpreterException ie) {
                    Debug.LogError(ie.DecoratedMessage);
                }
            }
        }

        void OnDestroy() {
            object destroyFunc = mScript.Globals[luaFuncOnDestroy];
            if(destroyFunc != null) {
                try {
                    mScript.Call(destroyFunc);
                }
                catch(InterpreterException ie) {
                    Debug.LogError(ie.DecoratedMessage);
                }
            }
        }

        void Awake() {
            if(loadOnAwake) {
                //only load if from file or text asset
                switch(scriptFrom) {
                    case LoadFrom.File:
                    case LoadFrom.TextAsset:
                        Load();
                        break;
                }
            }

            mIsAwake = true;
        }

        IEnumerator Start() {
            DynValue startFunc = mScript.Globals.Get(luaFuncStart);
            if(!startFunc.IsNilOrNan())
                yield return StartCoroutine(BehaviourModule.InvokeRoutine(mScript, this, startFunc));
        }
    }
}