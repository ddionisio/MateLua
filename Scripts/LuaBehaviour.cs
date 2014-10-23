#define MATE_LUA_TRACE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UniLua;

namespace M8.Lua {
    [AddComponentMenu("M8/Lua/Behaviour")]
    public class LuaBehaviour : MonoBehaviour {
        private const string luaMethodAwake = "awake";
        private const string luaMethodStart = "start";
        private const string luaMethodOnEnable = "onEnable";
        private const string luaMethodOnDisable = "onDisable";
        private const string luaMethodOnDestroy = "onDestroy";

        private const string luaMethodUpdate = "update";
        private const string luaMethodLateUpdate = "lateUpdate";
        private const string luaMethodFixedUpdate = "fixedUpdate";

        private const string luaMethodOnSpawned = "onSpawned";
        private const string luaMethodOnDespawned = "onDespawned";

        private const int nil = (int)LuaType.LUA_TNIL;

        public string scriptPath; //path to lua file
        public TextAsset scriptText; //if code path is empty, use this for loading

        private ILuaState mLua;

        private int mLuaMethodStart;
        private int mLuaMethodOnEnable;
        private int mLuaMethodOnDisable;
        private int mLuaMethodOnDestroy;

        private int mLuaMethodOnSpawned;
        private int mLuaMethodOnDespawned;

        public ILuaState lua { get { return mLua; } }

        //Mate Calls

        void OnSpawned() {
            if(mLuaMethodOnSpawned != nil)
                CallMethod(mLuaMethodOnSpawned);
        }

        void OnDespawned() {
            if(mLuaMethodOnDespawned != nil)
                CallMethod(mLuaMethodOnDespawned);
        }

        //Unity Calls
                
        void OnDestroy() {
            if(mLuaMethodOnDestroy != nil)
                CallMethod(mLuaMethodOnDestroy);
        }

        void OnEnable() {
            if(mLuaMethodOnEnable != nil)
                CallMethod(mLuaMethodOnEnable);
        }

        void OnDisable() {
            StopAllCoroutines();

            if(mLuaMethodOnDisable != nil)
                CallMethod(mLuaMethodOnDisable);
        }
                
        void Awake() {
            //init lua
            mLua = LuaAPI.NewState();
            mLua.L_OpenLibs();
                        
            //common unity libs
            mLua.L_RequireF(Library.LGameObject.LIB_NAME, Library.LGameObject.OpenLib, false);

            ThreadStatus status = string.IsNullOrEmpty(scriptPath) ? mLua.L_DoString(scriptText.text) : mLua.L_DoFile(scriptPath);
            if(status != ThreadStatus.LUA_OK) {
                throw new Exception(mLua.ToString(-1));
            }

            if(!mLua.IsTable(-1)) {
                throw new Exception("Lua script's return value is not a table.");
            }

            //grab callbacks from table
            int awakeInd = GetMethod(luaMethodAwake);

            mLuaMethodStart = GetMethod(luaMethodStart);
            mLuaMethodOnEnable = GetMethod(luaMethodOnEnable);
            mLuaMethodOnDisable = GetMethod(luaMethodOnDisable);
            mLuaMethodOnDestroy = GetMethod(luaMethodOnDestroy);

            mLuaMethodOnSpawned = GetMethod(luaMethodOnSpawned);
            mLuaMethodOnDespawned = GetMethod(luaMethodOnDespawned);

            int updateInd = GetMethod(luaMethodUpdate);
            if(updateInd != nil) {
                M8.Auxiliary.AuxUpdate aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxUpdate>(gameObject);
                aux.callback += delegate() { CallMethod(updateInd); };
            }

            int fixedUpdateInd = GetMethod(luaMethodFixedUpdate);
            if(fixedUpdateInd != nil) {
                M8.Auxiliary.AuxFixedUpdate aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxFixedUpdate>(gameObject);
                aux.callback += delegate() { CallMethod(fixedUpdateInd); };
            }

            int lateUpdateInd = GetMethod(luaMethodLateUpdate);
            if(lateUpdateInd != nil) {
                M8.Auxiliary.AuxLateUpdate aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxLateUpdate>(gameObject);
                aux.callback += delegate() { CallMethod(lateUpdateInd); };
            }

            //add some variables
            mLua.PushLightUserData(gameObject);
            mLua.SetGlobal("gameObject");

            mLua.PushLightUserData(transform);
            mLua.SetGlobal("transform");

            //add some functions
            mLua.PushCSharpFunction(LuaInvoke);
            mLua.SetGlobal("invoke");

            mLua.PushCSharpFunction(LuaInvokeRepeat);
            mLua.SetGlobal("invokeRepeat");

            mLua.PushCSharpFunction(LuaCancelInvoke);
            mLua.SetGlobal("cancelInvoke");

            mLua.PushCSharpFunction(LuaCancelAllInvoke);
            mLua.SetGlobal("cancelAllInvoke");
                                    
            /*mLua.CreateTable(0, 2);
            mLua.PushLightUserData(gameObject);
            mLua.SetField(-2, "__go");
            mLua.PushCSharpFunction(goname);
            mLua.SetField(-2, "name");*/

            mLua.Pop(1);
                                    
            //awake
            if(awakeInd != nil)
                CallMethod(awakeInd);
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
            if(mLuaMethodStart != nil)
                CallMethod(mLuaMethodStart);
        }

        //Internal

        /// <summary>
        /// Returns LuaType.LUA_TNIL if method is not found.
        /// </summary>
        private int GetMethod(string name) {
            mLua.GetField(-1, name);
            if(!mLua.IsFunction(-1)) {
                mLua.Pop(1);
                return nil;
            }

            return mLua.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        }

        private void CallMethod(int funcRef) {
            mLua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);

#if MATE_LUA_TRACE
            // insert `traceback' function
            var b = mLua.GetTop();
            mLua.PushCSharpFunction(Traceback);
            mLua.Insert(b);

            var status = mLua.PCall(0, 0, b);
#else
            var status = mLua.PCall(0, 0, 0);
#endif
            if(status != ThreadStatus.LUA_OK) {
                Debug.LogError(mLua.ToString(-1));
            }

#if MATE_LUA_TRACE
            // remove `traceback' function
            mLua.Remove(b);
#endif
        }

        private static int Traceback(ILuaState lua) {
            var msg = lua.ToString(1);
            if(msg != null) {
                lua.L_Traceback(lua, msg, 1);
            }
            // is there an error object?
            else if(!lua.IsNoneOrNil(1)) {
                // try its `tostring' metamethod
                if(!lua.L_CallMeta(1, "__tostring")) {
                    lua.PushString("(no error message)");
                }
            }
            return 1;
        }

        private int GetFuncRef(ILuaState lua, int ind) {
            int funcRef = 0;

            if(lua.IsFunction(ind)) {
                lua.PushValue(ind);
                funcRef = lua.L_Ref(LuaDef.LUA_REGISTRYINDEX);
            }
            else if(lua.IsString(ind)) {
                string f = lua.ToString(ind);
                lua.GetGlobal(f);
                if(lua.IsFunction(-1))
                    funcRef = lua.L_Ref(LuaDef.LUA_REGISTRYINDEX);
                else {
                    lua.Pop(1);
                    lua.L_ArgError(ind, "Function not found: "+f);
                }
            }
            else
                lua.L_ArgError(ind, "Argument is not a function or string.");

            return funcRef;
        }
                
        private int LuaInvoke(ILuaState lua) {

            int funcRef = GetFuncRef(lua, 1);
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

            int funcRef = GetFuncRef(lua, 1);
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