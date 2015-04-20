using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using MoonSharp.Interpreter;

namespace M8.Lua {
    using Modules;

    [AddComponentMenu("M8/Lua/Behaviour")]
    public class LuaBehaviour : MonoBehaviour {
        public enum LoadFrom {
            String,
            File,
            TextAsset,
        }

        public new string name;

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
        SerializedVariable[] properties; //added to the lua environment before executing script

        private Script mScript;
        private DynValue mScriptResult;

        private string mScriptString; //code from runtime

        private DynValue mScriptFuncEnable;
        private DynValue mScriptFuncDisable;

        private bool mIsAwake = false;
        private bool mIsRestart = false;

        public Script script { get { return mScript; } }

        public DynValue result { get { return mScriptResult; } }

        public bool isLoaded { get { return mScript != null; } }

        public void SetSourceFromString(string s) {
            scriptFrom = LoadFrom.String;
            scriptPath = "";
            scriptText = null;
            mScriptString = s;
        }

        public void SetSourceFromFile(string path) {
            scriptFrom = LoadFrom.File;
            scriptPath = path;
            scriptText = null;
            mScriptString = "";
        }

        public void SetSourceFromTextAsset(TextAsset text) {
            scriptFrom = LoadFrom.TextAsset;
            scriptPath = "";
            scriptText = text;
            mScriptString = "";
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
            CoreModules coreModules;
            UnityCoreModules unityCoreModules;
            MateCoreModules mateCoreModules;

            var settings = GlobalSettings.instance;
            if(settings) {
                coreModules = settings.coreModules;
                unityCoreModules = settings.unityCoreModules;
                mateCoreModules = settings.mateCoreModules;
            }
            else {
                coreModules = GlobalSettings.coreModuleDefault;
                unityCoreModules = GlobalSettings.unityCoreModuleDefault;
                mateCoreModules = 0;
            }
            
            mScript = new Script(coreModules);

            //add core modules
            mScript.Globals.RegisterUnityCoreModules(unityCoreModules);
            mScript.Globals.RegisterMateCoreModules(mateCoreModules);

            if(loaderOverride)
                mScript.Options.ScriptLoader = loaderOverride;

            //add component modules
            GameObjectModule.Register(mScript.Globals, gameObject, this);
            TransformModule.Register(mScript.Globals, transform);
            BehaviourModule.Register(mScript.Globals, this);

            //add properties
            if(properties != null) {
                Table propTable = new Table(mScript);
                mScript.Globals[Const.luaPropertiesTable] = propTable;

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

            mScriptFuncEnable = mScript.Globals.Get(Const.luaFuncOnEnable);
            mScriptFuncDisable = mScript.Globals.Get(Const.luaFuncOnDisable);

            //awake
            var awakeFunc = mScript.Globals.Get(Const.luaFuncAwake);
            if(awakeFunc.IsNotNil()) {
                try {
                    mScript.Call(awakeFunc);
                }
                catch(InterpreterException ie) {
                    Debug.LogError(ie.DecoratedMessage);
                }
            }

            //manually enable if we are instantiated before, and are active on scene
            if(mIsAwake) {
                mIsRestart = true;

                if(enabled && gameObject.activeInHierarchy)
                    OnEnable();
            }
        }

        void OnEnable() {
            if(mScriptFuncEnable != null && mScriptFuncEnable.IsNotNil()) {
                try {
                    mScript.Call(mScriptFuncEnable);
                }
                catch(InterpreterException ie) {
                    Debug.LogError(ie.DecoratedMessage);
                }

                if(mIsRestart) {
                    StartCoroutine(Start());
                    mIsRestart = false;
                }
            }
        }

        void OnDisable() {
            if(mScriptFuncDisable != null && mScriptFuncDisable.IsNotNil()) {
                try {
                    mScript.Call(mScriptFuncDisable);
                }
                catch(InterpreterException ie) {
                    Debug.LogError(ie.DecoratedMessage);
                }
            }
        }

        void OnDestroy() {
            if(mScript != null) {
                var destroyFunc = mScript.Globals.Get(Const.luaFuncOnDestroy);
                if(destroyFunc.IsNotNil()) {
                    try {
                        mScript.Call(destroyFunc);
                    }
                    catch(InterpreterException ie) {
                        Debug.LogError(ie.DecoratedMessage);
                    }
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
            if(mScript != null) {
                DynValue startFunc = mScript.Globals.Get(Const.luaFuncStart);
                if(startFunc.IsNotNil())
                    yield return StartCoroutine(BehaviourModule.InvokeRoutine(mScript, this, startFunc));
            }
        }
    }
}