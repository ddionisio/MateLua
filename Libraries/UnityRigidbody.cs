using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class UnityRigidbody {
        public const string META_NAME = "Unity.Rigidbody.Meta";
        public const string LIB_NAME = "Unity.Rigidbody";

        private static NameFuncPair[] m_funcs = null;
        private static NameFuncPair[] l_funcs = null;

        public static int OpenLib(ILuaState lua) {
            if(m_funcs == null)
                m_funcs = new NameFuncPair[] {
                    new NameFuncPair(Utils.GETTER, Get),
                    new NameFuncPair(Utils.SETTER, Set),

                    new NameFuncPair("compareTag", UnityComponent.CompareTag),
                    new NameFuncPair("getComponent", UnityComponent.GetComponent),
                    new NameFuncPair("getComponentInChildren", UnityComponent.GetComponentInChildren),
                    new NameFuncPair("getComponentInParent", UnityComponent.GetComponentInParent),

                    new NameFuncPair("sendMessage", UnityComponent.SendMessage),
                    new NameFuncPair("sendMessageUpwards", UnityComponent.SendMessageUpwards),
                    new NameFuncPair("broadcastMessage", UnityComponent.BroadcastMessage),
                };
            if(l_funcs == null)
                l_funcs = new NameFuncPair[] {
                    new NameFuncPair("Cast", Cast),
                };

            Utils.NewLibMetaGetterSetter(lua, META_NAME, m_funcs, l_funcs);

            return 1;
        }

        public static void Push(ILuaState lua, Rigidbody body) {
            if(body) {
                lua.NewUserData(body);
                lua.SetMetaTable(META_NAME);
            }
            else //null given,
                lua.PushNil();
        }

        private static int Cast(ILuaState lua) {
            lua.NewUserData(Utils.CheckUnityObject<Rigidbody>(lua, 1));
            lua.SetMetaTable(META_NAME);
            return 1;
        }

        private static int Get(ILuaState lua) {
            Rigidbody body = Utils.CheckUnityObject<Rigidbody>(lua, 1);
            string field = lua.L_CheckString(2);
            switch(field) {
                
                default:
                    if(!UnityComponent.PushField(lua, body, field))
                        if(!lua.L_GetMetaField(1, field))
                            lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 1;
        }

        private static int Set(ILuaState lua) {
            Rigidbody body = Utils.CheckUnityObject<Rigidbody>(lua, 1);
            string field = lua.L_CheckString(2);
            switch(field) {
                
                default:
                    if(!UnityComponent.SetField(lua, body, field))
                        lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 0;
        }
    }
}