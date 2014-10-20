﻿#define MATE_LUA_TRACE

using UnityEngine;
using System;
using System.Collections;

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
            if(mLuaMethodOnDisable != nil)
                CallMethod(mLuaMethodOnDisable);
        }
                
        void Awake() {
            //init lua
            mLua = LuaAPI.NewState();
            mLua.L_OpenLibs();

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
            
            mLua.Pop(1); //done with table

            //awake
            if(awakeInd != nil)
                CallMethod(awakeInd);
        }

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
                return (int)LuaType.LUA_TNIL;
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
    }
}