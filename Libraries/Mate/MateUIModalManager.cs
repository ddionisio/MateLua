using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    using UIModalManager = UIModal.Manager;

    public static class MateUIModalManager {
        public const string LIB_NAME = "Mate.UIModalManager";

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
                    new NameFuncPair("IsInStack", IsInStack),
                    new NameFuncPair("Replace", Replace),
                    new NameFuncPair("Open", Open),
                    new NameFuncPair("CloseTop", CloseTop),
                    new NameFuncPair("CloseAll", CloseAll),
                    new NameFuncPair("CloseUpTo", CloseUpTo),
                };

            Utils.NewMetaGetterSetter(lua, typeof(UIModalManager), m_funcs);
            lua.L_NewLib(l_funcs);

            Utils.SetMetaTableByType(lua, typeof(UIModalManager));

            return 1;
        }

        private static int Get(ILuaState lua) {
            string field = lua.L_CheckString(2);
            switch(field) {
                case "activeCount":
                    lua.PushInteger(UIModalManager.instance.activeCount);
                    break;
                case "top":
                    lua.PushString(UIModalManager.instance.ModalGetTop());
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
            lua.L_Error("Access Denied: {0}", field);
            return 0;
        }

        private static int IsInStack(ILuaState lua) {
            string modal = lua.L_CheckString(1);
            UIModalManager.instance.ModalIsInStack(modal);
            return 0;
        }

        private static int Replace(ILuaState lua) {
            string modal = lua.L_CheckString(1);
            UIModalManager.instance.ModalReplace(modal);
            return 0;
        }

        private static int Open(ILuaState lua) {
            string modal = lua.L_CheckString(1);
            UIModalManager.instance.ModalOpen(modal);
            return 0;
        }

        private static int CloseTop(ILuaState lua) {
            UIModalManager.instance.ModalCloseTop();
            return 0;
        }

        private static int CloseAll(ILuaState lua) {
            UIModalManager.instance.ModalCloseAll();
            return 0;
        }

        private static int CloseUpTo(ILuaState lua) {
            string modal = lua.L_CheckString(1);
            UIModalManager.instance.ModalCloseUpTo(modal, lua.GetTop() >= 2 ? lua.ToBoolean(2) : true);
            return 0;
        }
    }
}