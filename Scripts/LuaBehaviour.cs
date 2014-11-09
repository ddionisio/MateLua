using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UniLua;

namespace M8.Lua {
    [AddComponentMenu("M8/Lua/Behaviour")]
    public class LuaBehaviour : MonoBehaviour, ILuaInitializer {
        private const string luaMethodAwake = "awake";
        private const string luaMethodStart = "start";
        private const string luaMethodOnEnable = "onEnable";
        private const string luaMethodOnDisable = "onDisable";
        private const string luaMethodOnDestroy = "onDestroy";

        private const string luaMethodUpdate = "update";
        private const string luaMethodLateUpdate = "lateUpdate";
        private const string luaMethodFixedUpdate = "fixedUpdate";

        private const string luaMethodTriggerEnter = "onTriggerEnter";
        private const string luaMethodTriggerStay = "onTriggerStay";
        private const string luaMethodTriggerExit = "onTriggerExit";

        private const string luaMethodCollisionEnter = "onCollisionEnter";
        private const string luaMethodCollisionStay = "onCollisionStay";
        private const string luaMethodCollisionExit = "onCollisionExit";

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
                
        public string scriptPath; //path to lua file
        public TextAsset scriptText; //if code path is empty, use this for loading

        [HideInInspector]
        [SerializeField]
        public Variable[] initialVars; //added to the lua environment before executing script

        private ILuaState mLua;

        //unity stuff
        private int mAwakeInd;
        private int mLuaMethodStart;
        private int mLuaMethodOnEnable;
        private int mLuaMethodOnDisable;
        private int mLuaMethodOnDestroy;

        public ILuaState lua { get { return mLua; } }

        //Variable set/get from this lua's global

        public bool GetBoolean(string varName) {
            mLua.GetGlobal(varName);
            bool ret = mLua.ToBoolean(-1);
            mLua.Pop(1);
            return ret;
        }

        public void SetBoolean(string varName, bool val) {
            mLua.PushBoolean(val);
            mLua.SetGlobal(varName);
        }

        public float GetFloat(string varName, float defaultVal) {
            mLua.GetGlobal(varName);

            bool isNum;

            float ret = (float)mLua.ToNumberX(-1, out isNum);

            if(!isNum) ret = defaultVal;

            mLua.Pop(1);

            return ret;
        }

        public void SetFloat(string varName, float val) {
            mLua.PushNumber(val);
            mLua.SetGlobal(varName);
        }

        public int GetInt(string varName, int defaultVal) {
            mLua.GetGlobal(varName);

            bool isNum;

            int ret = mLua.ToIntegerX(-1, out isNum);

            if(!isNum) ret = defaultVal;

            mLua.Pop(1);

            return ret;
        }

        public void SetInt(string varName, int val) {
            mLua.PushInteger(val);
            mLua.SetGlobal(varName);
        }

        public string GetString(string varName, string defaultVal) {
            mLua.GetGlobal(varName);

            string ret = mLua.L_ToString(-1);

            if(ret == null) ret = defaultVal;

            mLua.Pop(1);

            return ret;
        }

        public void SetString(string varName, string val) {
            mLua.PushString(val);
            mLua.SetGlobal(varName);
        }

        public object GetObject(string varName, object defaultVal) {
            mLua.GetGlobal(varName);

            object ret = lua.ToUserData(-1);

            if(ret == null) ret = defaultVal;

            mLua.Pop(1);

            return ret;
        }

        /// <summary>
        /// Set val to null to 'delete' variable
        /// </summary>
        public void SetObject(string varName, object val) {
            if(val == null)
                mLua.PushNil();
            else {
                mLua.NewUserData(val);
                Utils.SetMetaTableByType(lua, val.GetType());
            }

            mLua.SetGlobal(varName);
        }

        public void Delete(string varName) {
            SetObject(varName, null);
        }

        //Unity Calls

        void OnDestroy() {
            if(mLuaMethodOnDestroy != Utils.nil)
                Utils.CallMethod(mLua, mLuaMethodOnDestroy);
        }

        void OnEnable() {
            if(mLuaMethodOnEnable != Utils.nil)
                Utils.CallMethod(mLua, mLuaMethodOnEnable);
        }

        void OnDisable() {
            StopAllCoroutines();

            if(mLuaMethodOnDisable != Utils.nil)
                Utils.CallMethod(mLua, mLuaMethodOnDisable);
        }

        void ILuaInitializer.LuaRequire(ILuaState lua) {
            //common unity libs
            lua.L_RequireF(Library.UnityObject.LIB_NAME, Library.UnityObject.OpenLib, false);
            lua.L_RequireF(Library.UnityTime.LIB_NAME, Library.UnityTime.OpenLib, false);
            lua.L_RequireF(Library.UnityGameObject.LIB_NAME, Library.UnityGameObject.OpenLib, false);
            lua.L_RequireF(Library.UnityComponent.LIB_NAME, Library.UnityComponent.OpenLib, false);
            lua.L_RequireF(Library.UnityBehaviour.LIB_NAME, Library.UnityBehaviour.OpenLib, false);
            lua.L_RequireF(Library.UnityTransform.LIB_NAME, Library.UnityTransform.OpenLib, false);
            lua.L_RequireF(Library.UnityCollider.LIB_NAME, Library.UnityCollider.OpenLib, false);
            lua.L_RequireF(Library.UnityBounds.LIB_NAME, Library.UnityBounds.OpenLib, false);
            lua.L_RequireF(Library.UnityRigidbody.LIB_NAME, Library.UnityRigidbody.OpenLib, false);
            lua.L_RequireF(Library.UnityCollision.LIB_NAME, Library.UnityCollision.OpenLib, false);

            //meta-only objects
            Library.UnityContactPoint.DefineMeta(lua);
        }

        void ILuaInitializer.LuaPreExecute(ILuaState lua) {
            //add some variables
            Library.UnityGameObject.Push(lua, gameObject);
            lua.SetGlobal("gameObject");

            Library.UnityTransform.Push(lua, transform);
            lua.SetGlobal("transform");

            //add some functions
            lua.PushCSharpFunction(LuaInvoke);
            lua.SetGlobal("invoke");

            lua.PushCSharpFunction(LuaInvokeRepeat);
            lua.SetGlobal("invokeRepeat");

            lua.PushCSharpFunction(LuaCancelInvoke);
            lua.SetGlobal("cancelInvoke");

            lua.PushCSharpFunction(LuaCancelAllInvoke);
            lua.SetGlobal("cancelAllInvoke");

            //add initial variables
            for(int i = 0; i < initialVars.Length; i++) {
                Variable var = initialVars[i];
                if(!string.IsNullOrEmpty(var.name)) {
                    switch(var.type) {
                        case Variable.Type.Boolean:
                            lua.PushBoolean(var.iVal != 0);
                            break;
                        case Variable.Type.Integer:
                            lua.PushInteger(var.iVal);
                            break;
                        case Variable.Type.Float:
                            lua.PushNumber(var.fVal);
                            break;
                        case Variable.Type.String:
                            lua.PushString(var.sVal);
                            break;
                        case Variable.Type.Object:
                            if(var.oVal) {
                                lua.NewUserData(var.oVal);
                                Utils.SetMetaTableByType(mLua, var.oVal.GetType());
                            }
                            else
                                continue;
                            break;
                    }
                    lua.SetGlobal(var.name);
                }
            }
        }

        void ILuaInitializer.LuaPostExecute(ILuaState lua) {
            //grab callbacks from table
            mAwakeInd = Utils.GetMethod(lua, luaMethodAwake);

            mLuaMethodStart = Utils.GetMethod(lua, luaMethodStart);
            mLuaMethodOnEnable = Utils.GetMethod(lua, luaMethodOnEnable);
            mLuaMethodOnDisable = Utils.GetMethod(lua, luaMethodOnDisable);
            mLuaMethodOnDestroy = Utils.GetMethod(lua, luaMethodOnDestroy);

            int updateInd = Utils.GetMethod(lua, luaMethodUpdate);
            if(updateInd != Utils.nil) {
                M8.Auxiliary.AuxUpdate aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxUpdate>(gameObject);
                aux.callback += delegate() { Utils.CallMethod(lua, updateInd); };
            }

            int fixedUpdateInd = Utils.GetMethod(lua, luaMethodFixedUpdate);
            if(fixedUpdateInd != Utils.nil) {
                M8.Auxiliary.AuxFixedUpdate aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxFixedUpdate>(gameObject);
                aux.callback += delegate() { Utils.CallMethod(lua, fixedUpdateInd); };
            }

            int lateUpdateInd = Utils.GetMethod(lua, luaMethodLateUpdate);
            if(lateUpdateInd != Utils.nil) {
                M8.Auxiliary.AuxLateUpdate aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxLateUpdate>(gameObject);
                aux.callback += delegate() { Utils.CallMethod(lua, lateUpdateInd); };
            }

            int triggerEnterInd = Utils.GetMethod(lua, luaMethodTriggerEnter);
            int triggerStayInd = Utils.GetMethod(lua, luaMethodTriggerStay);
            int triggerExitInd = Utils.GetMethod(lua, luaMethodTriggerExit);
            if(triggerEnterInd != Utils.nil || triggerStayInd != Utils.nil || triggerExitInd != Utils.nil) {
                M8.Auxiliary.AuxTrigger aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxTrigger>(gameObject);
                if(triggerEnterInd != Utils.nil) aux.enterCallback += delegate(Collider c) { Utils.CallMethod<Collider>(lua, triggerEnterInd, c); };
                if(triggerStayInd != Utils.nil) aux.stayCallback += delegate(Collider c) { Utils.CallMethod<Collider>(lua, triggerStayInd, c); };
                if(triggerExitInd != Utils.nil) aux.exitCallback += delegate(Collider c) { Utils.CallMethod<Collider>(lua, triggerExitInd, c); };
            }

            int collEnterInd = Utils.GetMethod(lua, luaMethodCollisionEnter);
            int collStayInd = Utils.GetMethod(lua, luaMethodCollisionStay);
            int collExitInd = Utils.GetMethod(lua, luaMethodCollisionExit);
            if(collEnterInd != Utils.nil || collStayInd != Utils.nil || collExitInd != Utils.nil) {
                M8.Auxiliary.AuxCollision aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxCollision>(gameObject);
                if(collEnterInd != Utils.nil) aux.enterCallback += delegate(Collision c) { Utils.CallMethod<Collision>(lua, collEnterInd, c); };
                if(collStayInd != Utils.nil) aux.stayCallback += delegate(Collision c) { Utils.CallMethod<Collision>(lua, collStayInd, c); };
                if(collExitInd != Utils.nil) aux.exitCallback += delegate(Collision c) { Utils.CallMethod<Collision>(lua, collExitInd, c); };
            }
        }

        void Awake() {
            //grab lua initializers
            Component[] comps = GetComponents<Component>();
            List<ILuaInitializer> inits = new List<ILuaInitializer>(comps.Length);
            for(int i = 0; i < comps.Length; i++) {
                ILuaInitializer ilinit = comps[i] as ILuaInitializer;
                if(ilinit != null)
                    inits.Add(ilinit);
            }

            //init lua
            mLua = LuaAPI.NewState();
            mLua.L_OpenLibs();

            //setup requires
            for(int i = 0; i < inits.Count; i++)
                inits[i].LuaRequire(mLua);

            mLua.Pop(mLua.GetTop());

            //setup global fields and functions
            for(int i = 0; i < inits.Count; i++)
                inits[i].LuaPreExecute(mLua);

            //execute
            ThreadStatus status = string.IsNullOrEmpty(scriptPath) ? mLua.L_DoString(scriptText.text) : mLua.L_DoFile(scriptPath);
            if(status != ThreadStatus.LUA_OK) {
                throw new Exception(mLua.ToString(-1));
            }

            if(!mLua.IsTable(-1)) {
                throw new Exception("Lua script's return value is not a table.");
            }

            //additional setups
            for(int i = 0; i < inits.Count; i++)
                inits[i].LuaPostExecute(mLua);

            /*mLua.CreateTable(0, 2);
            mLua.PushLightUserData(gameObject);
            mLua.SetField(-2, "__go");
            mLua.PushCSharpFunction(goname);
            mLua.SetField(-2, "name");*/

            //awake
            if(mAwakeInd != Utils.nil)
                Utils.CallMethod(mLua, mAwakeInd);
        }

        /*static int goname(ILuaState l) {
            l.SetTop(1);
            l.L_CheckType(1, LuaType.LUA_TTABLE);
                        
            l.GetField(1, "__go");
            GameObject go = l.ToUserData(-1) as GameObject;

            l.Pop(1);

            l.PushString(go.name);

            return 1;
        }*/

        // Use this for initialization
        void Start() {
            if(mLuaMethodStart != Utils.nil)
                Utils.CallMethod(mLua, mLuaMethodStart);
        }

        //Internal

        private int LuaInvoke(ILuaState lua) {

            int funcRef = Utils.GetFuncRef(lua, 1);
            if(funcRef == 0) return 0;

            float time = (float)lua.L_CheckNumber(2);

            //TODO: n-arguments to pass
            //Debug.Log("narg: "+lua.GetTop());
            IEnumerator e = DoInvoke(lua, funcRef, time);

            StartCoroutine(e);

            lua.PushLightUserData(e); //keep this if you want to cancel invoke
            return 1;
        }

        private int LuaInvokeRepeat(ILuaState lua) {

            int funcRef = Utils.GetFuncRef(lua, 1);
            if(funcRef == 0) return 0;

            float time = (float)lua.L_CheckNumber(2);
            float repeat = (float)lua.L_CheckNumber(3);

            //TODO: n-arguments to pass
            //Debug.Log("narg: "+lua.GetTop());
            IEnumerator e = DoInvokeRepeat(lua, funcRef, time, repeat);

            StartCoroutine(e);

            lua.PushLightUserData(e); //keep this if you want to cancel invoke
            return 1;
        }

        private int LuaCancelInvoke(ILuaState lua) {
            IEnumerator e = lua.ToUserData(1) as IEnumerator;
            if(e != null)
                StopCoroutine(e);
            return 0;
        }

        private int LuaCancelAllInvoke(ILuaState lua) {
            StopAllCoroutines();
            return 0;
        }

        private IEnumerator DoInvoke(ILuaState lua, int funcRef, float time) {
            if(time > 0)
                yield return new WaitForSeconds(time);
            else
                yield return null;

            lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
            ThreadStatus status = lua.PCall(0, 0, 0);
            if(status != ThreadStatus.LUA_OK)
                lua.L_Error("Error running function: "+lua.L_ToString(-1));
        }

        private IEnumerator DoInvokeRepeat(ILuaState lua, int funcRef, float time, float repeatRate) {
            if(time > 0)
                yield return new WaitForSeconds(time);
            else
                yield return null;

            WaitForSeconds wait = new WaitForSeconds(repeatRate);
            while(true) {
                lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
                ThreadStatus status = lua.PCall(0, 0, 0);
                if(status != ThreadStatus.LUA_OK) {
                    lua.L_Error("Error running function: "+lua.L_ToString(-1));
                    break;
                }
                yield return wait;
            }
        }
    }
}