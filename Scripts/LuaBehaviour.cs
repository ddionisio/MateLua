using UnityEngine;
using System;
using System.Collections;

using UniLua;

namespace M8.Lua {
    [AddComponentMenu("M8/Lua/Behaviour")]
    public class LuaBehaviour : MonoBehaviour {
        private const string luaMethodAwake = "Awake";
        private const string luaMethodStart = "Start";

        public string scriptPath; //path to lua file
        public TextAsset scriptText; //if code path is empty, use this for loading

        private ILuaState mLua;

        private int mLuaMethodStart;

        public ILuaState lua { get { return mLua; } }

        void Awake() {
            //init lua
            mLua = LuaAPI.NewState();
            mLua.L_OpenLibs();

            ThreadStatus status = string.IsNullOrEmpty(scriptPath) ? mLua.L_DoString(scriptText.text) : mLua.L_DoFile(scriptPath);
            if(status != ThreadStatus.LUA_OK) {
                throw new Exception(mLua.ToString(-1));
            }

            if(!mLua.IsTable(-1)) {
                throw new Exception("Framework main's return value is not a table.");
            }

            //store callbacks
            int awakeInd = GetMethod(luaMethodAwake);

            mLuaMethodStart = GetMethod(luaMethodStart);

            mLua.Pop(1);

            //awake
            if(awakeInd == (int)LuaType.LUA_TNIL)
                CallMethod(awakeInd);
        }

        // Use this for initialization
        void Start() {
            if(mLuaMethodStart == (int)LuaType.LUA_TNIL)
                CallMethod(mLuaMethodStart);
        }

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