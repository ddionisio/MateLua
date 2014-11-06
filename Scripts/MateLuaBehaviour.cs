using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UniLua;

namespace M8.Lua {
    [AddComponentMenu("M8/Lua/MateBehaviour")]
    [RequireComponent(typeof(LuaBehaviour))]
    public class MateLuaBehaviour : MonoBehaviour, ILuaInitializer {
        private const string luaMethodOnSpawned = "onSpawned";
        private const string luaMethodOnDespawned = "onDespawned";

        [System.Serializable]
        public struct Include {
            public bool input; //set true to enable input
            public bool sceneManager;
            public bool sceneState;
        }

        public Include include;

        private ILuaState mLua;

        private int mLuaMethodOnSpawned;
        private int mLuaMethodOnDespawned;

        //Mate Calls

        void OnSpawned() {
            if(mLuaMethodOnSpawned != Utils.nil)
                Utils.CallMethod(mLua, mLuaMethodOnSpawned);
        }

        void OnDespawned() {
            if(mLuaMethodOnDespawned != Utils.nil)
                Utils.CallMethod(mLua, mLuaMethodOnDespawned);
        }

        void ILuaInitializer.LuaRequire(ILuaState lua) {
            mLua = lua;

            if(include.input) lua.L_RequireF(Library.MateInput.LIB_NAME, Library.MateInput.OpenLib, false);
            if(include.sceneManager) lua.L_RequireF(Library.MateSceneMgr.LIB_NAME, Library.MateSceneMgr.OpenLib, false);
            if(include.sceneState) {
                lua.L_RequireF(Library.MateSceneState.LIB_NAME, Library.MateSceneState.OpenLib, false);
                lua.L_RequireF(Library.MateGlobalState.LIB_NAME, Library.MateGlobalState.OpenLib, false);
            }
        }

        void ILuaInitializer.LuaPreExecute(ILuaState lua) {

        }

        void ILuaInitializer.LuaPostExecute(ILuaState lua) {
            mLuaMethodOnSpawned = Utils.GetMethod(lua, luaMethodOnSpawned);
            mLuaMethodOnDespawned = Utils.GetMethod(lua, luaMethodOnDespawned);
        }
    }
}