using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class MateUserData {
        public const string LIB_NAME = "Mate.UserData";

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
                    new NameFuncPair("SetInt", SetInt),

                    new NameFuncPair("SnapshotSave", SnapshotSave),
                    new NameFuncPair("SnapshotRestore", SnapshotRestore),
                    new NameFuncPair("SnapshotDelete", SnapshotDelete),
                    new NameFuncPair("SnapshotPreserve", SnapshotPreserve),

                    new NameFuncPair("Load", Load),
                    new NameFuncPair("Save", Save),
                    new NameFuncPair("DeleteAll", DeleteAll),
                    new NameFuncPair("DeleteAllByNameContain", DeleteAllByNameContain),
                };

            Utils.NewMetaGetterSetter(lua, typeof(UserData), m_funcs);
            lua.L_NewLib(l_funcs);

            Utils.SetMetaTableByType(lua, typeof(UserData));

            return 1;
        }

        private static int Get(ILuaState lua) {
            string field = lua.L_CheckString(2);

            UserData ud = UserData.instance;
            System.Type type = ud.GetType(field);
            if(type != null) {
                if(type == typeof(int))
                    lua.PushInteger(ud.GetInt(field));
                else if(type == typeof(float))
                    lua.PushNumber(ud.GetFloat(field));
                else if(type == typeof(string))
                    lua.PushString(ud.GetString(field));
            }
            else if(!lua.L_GetMetaField(1, field))
                lua.PushNil();

            return 1;
        }

        private static int Set(ILuaState lua) {
            string field = lua.L_CheckString(2);

            UserData ud = UserData.instance;

            if(lua.Type(3) == LuaType.LUA_TSTRING)
                ud.SetString(field, lua.ToString(3));
            else if(lua.Type(3) == LuaType.LUA_TNUMBER)
                ud.SetFloat(field, (float)lua.ToNumber(3));
            else if(lua.IsNil(3))
                ud.Delete(field);
            else
                lua.L_ArgError(3, "Not a number or string.");

            return 0;
        }

        private static int SetInt(ILuaState lua) {
            string field = lua.L_CheckString(1);
            UserData.instance.SetInt(field, lua.L_CheckInteger(2));
            return 0;
        }

        private static int SnapshotSave(ILuaState lua) {
            UserData.instance.SnapshotSave();
            return 0;
        }

        private static int SnapshotRestore(ILuaState lua) {
            UserData.instance.SnapshotRestore();
            return 0;
        }

        private static int SnapshotDelete(ILuaState lua) {
            UserData.instance.SnapshotDelete();
            return 0;
        }

        private static int SnapshotPreserve(ILuaState lua) {
            string field = lua.L_CheckString(1);
            UserData.instance.SnapshotPreserve(field);
            return 0;
        }

        private static int Load(ILuaState lua) {
            UserData.instance.Load();
            return 0;
        }

        private static int Save(ILuaState lua) {
            UserData.instance.Save();
            return 0;
        }

        private static int DeleteAll(ILuaState lua) {
            UserData.instance.Delete();
            return 0;
        }

        private static int DeleteAllByNameContain(ILuaState lua) {
            string criteria = lua.L_CheckString(1);
            UserData.instance.DeleteAllByNameContain(criteria);
            return 0;
        }
    }
}