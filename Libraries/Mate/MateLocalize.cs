using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class MateLocalize {
        public const string LIB_NAME = "Mate.Localize";

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
                    new NameFuncPair("Refresh", Refresh),
                    new NameFuncPair("RegisterParam", RegisterParam),
                };

            Utils.NewMetaGetterSetter(lua, typeof(Localize), m_funcs);
            lua.L_NewLib(l_funcs);

            Utils.SetMetaTableByType(lua, typeof(Localize));

            return 1;
        }

        private static int Get(ILuaState lua) {
            string field = lua.L_CheckString(2);

            string text = Localize.instance.GetText(field);
            if(string.IsNullOrEmpty(text)) {
                if(!lua.L_GetMetaField(1, field))
                    lua.PushNil();
            }
            else
                lua.PushString(text);

            return 1;
        }

        private static int Set(ILuaState lua) {
            string field = lua.L_CheckString(2);
            lua.L_Error("Access denied {0}", field);
            return 0;
        }

        private static int Refresh(ILuaState lua) {
            Localize.instance.Refresh();
            return 0;
        }

        private static int RegisterParam(ILuaState lua) {
            string paramKey = lua.L_CheckString(1);
            int funcRef = Utils.GetFuncRef(lua, 2);
            if(funcRef > 0) {
                Localize.ParameterCallback call = delegate(string key) {
                    lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);

                    lua.PushString(key);

                    ThreadStatus status = lua.PCall(1, 1, 0);
                    if(status != ThreadStatus.LUA_OK)
                        lua.L_Error("Error running function: "+lua.L_ToString(-1));

                    return lua.ToString(-1);
                };

                Localize.instance.RegisterParam(paramKey, call);
            }
            return 0;
        }
    }
}