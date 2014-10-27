using UnityEngine;

using UniLua;

namespace M8.Lua {
    public struct Utils {
        public const string GETTER = "get";
        public const string SETTER = "set";

        public static T CheckUnityObject<T>(ILuaState lua, int ind) where T : Object {
            T obj = lua.ToUserData(ind) as T;
            if(obj == null)
                lua.L_ArgError(ind, "Not a "+typeof(T));
            return obj;
        }

        /// <summary>
        /// Generate the table with given meta.
        /// </summary>
        public static void NewLibMeta(ILuaState lua, string metaName, NameFuncPair[] metaFuncs, NameFuncPair[] libFuncs) {
            lua.NewMetaTable(metaName);

            lua.PushString("__index");
            lua.PushValue(-2); //meta
            lua.SetTable(-3); //meta.__index = meta

            lua.L_SetFuncs(metaFuncs, 0);

            lua.L_NewLib(libFuncs);
        }

        public static void NewMetaGetterSetter(ILuaState lua, string metaName, NameFuncPair[] metaFuncs) {
            lua.NewMetaTable(metaName);
            lua.L_SetFuncs(metaFuncs, 0);

            lua.PushString("__index");
            lua.PushString(GETTER);
            lua.GetTable(-3); //lib.getter
            lua.SetTable(-3); //meta.__index = lib.getter

            lua.PushString("__newindex");
            lua.PushString(SETTER);
            lua.GetTable(-3); //lib.setter
            lua.SetTable(-3); //meta.__newindex = lib.setter
        }

        public static void NewLibMetaGetterSetter(ILuaState lua, string metaName, NameFuncPair[] metaFuncs, NameFuncPair[] libFuncs) {
            NewMetaGetterSetter(lua, metaName, metaFuncs);

            lua.L_NewLib(libFuncs);
        }
    }
}