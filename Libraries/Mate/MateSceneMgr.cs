using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class MateSceneMgr {
        public const string LIB_NAME = "Mate.SceneManager";

        private static NameFuncPair[] m_funcs = null;
        private static NameFuncPair[] l_funcs = null;

        public static int OpenLib(ILuaState lua) {
            if(m_funcs == null)
                m_funcs = new NameFuncPair[] {
                    new NameFuncPair(Utils.GETTER, Get),
                    new NameFuncPair(Utils.SETTER, Set),
                };
            if(l_funcs == null)
                l_funcs = new NameFuncPair[] {
                    new NameFuncPair("LoadSceneNoTransition", LoadSceneNoTransition),
                    new NameFuncPair("LoadScene", LoadScene),
                    new NameFuncPair("LoadLevel", LoadLevel),
                    new NameFuncPair("Reload", Reload),
                    new NameFuncPair("Pause", Pause),
                    new NameFuncPair("Resume", Resume),
                };

            Utils.NewMetaGetterSetter(lua, typeof(SceneManager), m_funcs);
            lua.L_NewLib(l_funcs);

            Utils.SetMetaTableByType(lua, typeof(SceneManager));

            return 1;
        }

        private static int Get(ILuaState lua) {
            string field = lua.L_CheckString(2);
            switch(field) {
                case "isPaused":
                    lua.PushBoolean(SceneManager.instance.isPaused);
                    break;
                case "curLevel":
                    lua.PushInteger(SceneManager.instance.curLevel);
                    break;
                case "curLevelName":
                    lua.PushString(SceneManager.instance.curLevelName);
                    break;
                default:
                    if(!lua.L_GetMetaField(1, field))
                        lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 1;
        }

        private static int Set(ILuaState lua) {
            string field = lua.L_CheckString(2);
            switch(field) {
                default:
                    lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 0;
        }

        private static int LoadSceneNoTransition(ILuaState lua) {
            string scene = lua.L_CheckString(1);
            SceneManager.instance.LoadSceneNoTransition(scene);
            return 0;
        }

        private static int LoadScene(ILuaState lua) {
            string scene = lua.L_CheckString(1);

            int nargs = lua.GetTop();
            if(nargs == 1)
                SceneManager.instance.LoadScene(scene);
            else if(nargs == 2)
                SceneManager.instance.LoadScene(scene, lua.L_CheckString(2), null);
            if(lua.GetTop() == 3)
                SceneManager.instance.LoadScene(scene, lua.L_CheckString(2), lua.L_CheckString(3));

            return 0;
        }

        private static int LoadLevel(ILuaState lua) {
            int lvl = lua.L_CheckInteger(1);

            int nargs = lua.GetTop();
            if(nargs == 1)
                SceneManager.instance.LoadLevel(lvl);
            else if(nargs == 2)
                SceneManager.instance.LoadLevel(lvl, lua.L_CheckString(2), null);
            if(lua.GetTop() == 3)
                SceneManager.instance.LoadLevel(lvl, lua.L_CheckString(2), lua.L_CheckString(3));

            return 0;
        }

        private static int Reload(ILuaState lua) {
            int nargs = lua.GetTop();
            if(nargs == 0)
                SceneManager.instance.Reload();
            else if(nargs == 1)
                SceneManager.instance.Reload(lua.L_CheckString(1), null);
            else if(nargs == 2)
                SceneManager.instance.Reload(lua.L_CheckString(1), lua.L_CheckString(2));

            return 0;
        }

        private static int Pause(ILuaState lua) {
            SceneManager.instance.Pause();
            return 0;
        }

        private static int Resume(ILuaState lua) {
            SceneManager.instance.Resume();
            return 0;
        }
    }
}