using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class MatePool {
        private static NameFuncPair[] m_funcs = null;

        public static int DefineMeta(ILuaState lua) {
            if(m_funcs == null)
                m_funcs = new NameFuncPair[] {
                    new NameFuncPair(Utils.GETTER, Get),
                    new NameFuncPair(Utils.SETTER, Set),

                    new NameFuncPair("spawn", Spawn),
                    new NameFuncPair("release", Release),
                    new NameFuncPair("releaseAll", ReleaseAll),
                    new NameFuncPair("releaseAllByType", ReleaseAllByType),
                };

            Utils.NewMetaGetterSetter(lua, typeof(PoolController), m_funcs);

            return 1;
        }

        private static int Get(ILuaState lua) {
            PoolController pool = Utils.CheckUnityObject<PoolController>(lua, 1);
            string field = lua.L_CheckString(2);

            if(!UnityObject.PushField(lua, pool, field))
                if(!lua.L_GetMetaField(1, field))
                    lua.L_Error("Unknown field: {0}", field);
            return 1;
        }

        private static int Set(ILuaState lua) {
            string field = lua.L_CheckString(2);
            lua.L_Error("Access denied: {0}", field);
            return 0;
        }

        private static int Spawn(ILuaState lua) {
            PoolController pool = Utils.CheckUnityObject<PoolController>(lua, 1);
            string type = lua.L_CheckString(2);
            string name = lua.GetTop() >= 3 ? lua.L_CheckString(3) : null;
            Transform parent = lua.GetTop() >= 4 ? Utils.CheckUnityObject<Transform>(lua, 4) : null;

            Transform spawn = pool.Spawn(type, name, parent);
            UnityTransform.Push(lua, spawn);

            return 1;
        }

        private static int Release(ILuaState lua) {
            PoolController pool = Utils.CheckUnityObject<PoolController>(lua, 1);
            Transform entity = Utils.CheckUnityObject<Transform>(lua, 2);
            pool.Release(entity);
            return 0;
        }

        private static int ReleaseAll(ILuaState lua) {
            PoolController pool = Utils.CheckUnityObject<PoolController>(lua, 1);
            pool.ReleaseAll();
            return 0;
        }

        private static int ReleaseAllByType(ILuaState lua) {
            PoolController pool = Utils.CheckUnityObject<PoolController>(lua, 1);
            string type = lua.L_CheckString(2);
            pool.ReleaseAllByType(type);
            return 0;
        }
    }

    public static class MatePools {
        public const string LIB_NAME = "Mate.Pool";

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
                    new NameFuncPair("Spawn", Spawn),
                    new NameFuncPair("Release", Release),
                };

            Utils.NewMetaGetterSetter(lua, typeof(MateSceneState), m_funcs);
            lua.L_NewLib(l_funcs);

            Utils.SetMetaTableByType(lua, typeof(MateSceneState));

            return 1;
        }

        private static int Get(ILuaState lua) {
            string field = lua.L_CheckString(2);

            PoolController pool = PoolController.GetPool(field);
            if(pool) {
                lua.NewUserData(pool);
                Utils.SetMetaTableByType(lua, typeof(PoolController));
            }
            else if(!lua.L_GetMetaField(1, field))
                lua.PushNil();

            return 1;
        }

        private static int Set(ILuaState lua) {
            string field = lua.L_CheckString(2);
            lua.L_Error("Access denied: {0}", field);
            return 0;
        }

        private static int Spawn(ILuaState lua) {
            string group = lua.L_CheckString(1);
            string type = lua.L_CheckString(2);
            string name = lua.GetTop() >= 3 ? lua.L_CheckString(3) : null;
            Transform parent = lua.GetTop() >= 4 ? Utils.CheckUnityObject<Transform>(lua, 4) : null;

            Transform spawn = PoolController.Spawn(group, type, name, parent);
            UnityTransform.Push(lua, spawn);

            return 1;
        }

        private static int Release(ILuaState lua) {
            LuaType ltype = lua.Type(1);
            if(ltype == LuaType.LUA_TLIGHTUSERDATA || ltype == LuaType.LUA_TUSERDATA) {
                Transform entity = Utils.CheckUnityObject<Transform>(lua, 1);
                PoolController.ReleaseAuto(entity);
            }
            else {
                string group = lua.L_CheckString(1);
                Transform entity = Utils.CheckUnityObject<Transform>(lua, 2);
                PoolController.ReleaseByGroup(group, entity);
            }
            return 0;
        }
    }
}