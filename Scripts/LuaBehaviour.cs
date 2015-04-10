using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using MoonSharp.Interpreter;

namespace M8.Lua {
    using Modules;

    [AddComponentMenu("M8/Lua/Behaviour")]
    public class LuaBehaviour : MonoBehaviour {
        public const string luaFuncAwake = "Awake";
        public const string luaFuncStart = "Start";
        public const string luaFuncOnEnable = "OnEnable";
        public const string luaFuncOnDisable = "OnDisable";
        public const string luaFuncOnDestroy = "OnDestroy";

        [Serializable]
        public struct Variable {
            public enum Type {
                Boolean,
                Integer,
                Float,
                String,
                Object
            }

            public string name;
            public Type type;

            public int iVal;
            public float fVal;
            public string sVal;
            public UnityEngine.Object oVal;

            public void Reset() {
                iVal = 0;
                fVal = 0f;
                sVal = "";
                oVal = null;
            }
        }

        public LoaderBase loaderOverride;

        [Tooltip("Path to lua file, loaded via scriptLoader.")]
        public string scriptPath;

        [Tooltip("If scriptPath is empty, this is used for loading")]
        public TextAsset scriptText;

        [HideInInspector]
        public Variable[] initialVars; //added to the lua environment before executing script

        private Script mScript;
        private DynValue mScriptResult;

        private object mScriptFuncEnable;
        private object mScriptFuncDisable;

        public Script script { get { return mScript; } }

        public DynValue result { get { return mScriptResult; } }

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
            CoreModules coreModules = GlobalSettings.instance ? GlobalSettings.instance.coreModules : GlobalSettings.coreModuleDefault;
            UnityCoreModules unityCoreModules = GlobalSettings.instance ? GlobalSettings.instance.unityCoreModules : GlobalSettings.unityCoreModuleDefault;

            mScript = new Script(coreModules);
            mScript.Globals.RegisterUnityCoreModules(unityCoreModules);

            if(loaderOverride)
                mScript.Options.ScriptLoader = loaderOverride;
            
            //add game object and transform
            GameObjectModule.Register(mScript.Globals, gameObject);
            TransformModule.Register(mScript.Globals, transform);
            BehaviourModule.Register(mScript.Globals, this);
            
            //grab components for interfaces
            Component[] comps = GetComponentsInChildren<Component>(true);

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

            try {
                //load and excecute
                if(!string.IsNullOrEmpty(scriptPath))
                    mScriptResult = mScript.DoFile(scriptPath);
                else if(scriptText)
                    mScriptResult = mScript.DoString(scriptText.text);
            }
            catch(InterpreterException ie) {
                Debug.LogError(ie.DecoratedMessage);
            }

            for(int i = 0; i < comps.Length; i++) {
                //postload setup
                IInitialization m = comps[i] as IInitialization;
                if(m != null)
                    m.PostLoad(mScript);
            }
            
            mScriptFuncEnable = mScript.Globals[luaFuncOnEnable];
            mScriptFuncDisable = mScript.Globals[luaFuncOnDisable];

            object awakeFunc = mScript.Globals[luaFuncAwake];
            if(awakeFunc != null) {
                try {
                    mScript.Call(awakeFunc);
                }
                catch(InterpreterException ie) {
                    Debug.LogError(ie.DecoratedMessage);
                }
            }
        }

        IEnumerator Start() {
            DynValue startFunc = mScript.Globals.Get(luaFuncStart);
            if(!startFunc.IsNilOrNan())
                yield return StartCoroutine(BehaviourModule.InvokeRoutine(mScript, this, startFunc));
        }
    }
}