using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using MoonSharp.Interpreter;

namespace M8.Lua {
    using Modules;

    public class LuaBehaviour : MonoBehaviour, IDebugPrint {
        public const string luaFuncAwake = "Awake";
        public const string luaFuncStart = "Start";
        public const string luaFuncOnEnable = "OnEnable";
        public const string luaFuncOnDisable = "OnDisable";
        public const string luaFuncOnDestroy = "OnDestroy";

        public const CoreModules coreModuleDefault = CoreModules.Basic | CoreModules.Bit32 | CoreModules.Coroutine | CoreModules.Dynamic | CoreModules.ErrorHandling | CoreModules.LoadMethods | CoreModules.Math | CoreModules.Metatables | CoreModules.OS_Time | CoreModules.String | CoreModules.Table | CoreModules.TableIterators;
        public const UnityCoreModules unityCoreModuleDefault = UnityCoreModules.Time | UnityCoreModules.Math;

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

        public LoaderBase scriptLoader;

        [Tooltip("Path to lua file, loaded via scriptLoader.")]
        public string scriptPath;

        [Tooltip("If scriptPath is empty, this is used for loading")]
        public TextAsset scriptText;

        [HideInInspector]
        public CoreModules coreModules = coreModuleDefault;

        [HideInInspector]
        public UnityCoreModules unityCoreModules = unityCoreModuleDefault;

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
            mScript = new Script(coreModules);

            if(scriptLoader)
                mScript.Options.ScriptLoader = scriptLoader;

            //core modules
            mScript.Globals.RegisterUnityCoreModules(unityCoreModules);

            //add game object and transform
            GameObjectModule.Register(mScript.Globals, gameObject);
            TransformModule.Register(mScript.Globals, transform);


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

        void Start() {
            object startFunc = mScript.Globals[luaFuncStart];
            if(startFunc != null) {
                try {
                    mScript.Call(startFunc);
                }
                catch(InterpreterException ie) {
                    Debug.LogError(ie.DecoratedMessage);
                }
            }
        }

        void IDebugPrint.Print(string message) {
            Debug.Log(message);
        }
    }
}