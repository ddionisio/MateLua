using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class UnityCollider {
        public const string LIB_NAME = "Unity.Collider";

        public static int OpenLib(ILuaState lua) {
            NameFuncPair[] funcs = new NameFuncPair[] {
                new NameFuncPair("GetRigidbody", GetRigidbody),
                new NameFuncPair("IsEnabled", IsEnabled),
                new NameFuncPair("SetEnabled", SetEnabled),
                new NameFuncPair("IsTrigger", IsTrigger),
                new NameFuncPair("SetAsTrigger", SetAsTrigger),
                new NameFuncPair("GetBounds", GetBounds),
            };

            lua.L_NewLib(funcs);
            return 1;
        }

        private static int GetRigidbody(ILuaState lua) {
            Collider coll = Utils.CheckUnityObject<Collider>(lua, 1);
            Rigidbody body = coll.attachedRigidbody;
            if(body)
                lua.PushLightUserData(body);
            else
                lua.PushNil();
            return 1;
        }

        /// <summary>
        /// Check if given Behaviour is enabled. bool IsEnabled(behaviour)
        /// </summary>
        private static int IsEnabled(ILuaState lua) {
            Collider coll = Utils.CheckUnityObject<Collider>(lua, 1);
            lua.PushBoolean(coll.enabled);
            return 1;
        }

        /// <summary>
        /// Set Behaviour enabled. SetEnabled(behaviour)
        /// </summary>
        private static int SetEnabled(ILuaState lua) {
            Collider coll = Utils.CheckUnityObject<Collider>(lua, 1);
            coll.enabled = lua.ToBoolean(2);
            return 0;
        }

        private static int IsTrigger(ILuaState lua) {
            Collider coll = Utils.CheckUnityObject<Collider>(lua, 1);
            lua.PushBoolean(coll.isTrigger);
            return 1;
        }

        private static int SetAsTrigger(ILuaState lua) {
            Collider coll = Utils.CheckUnityObject<Collider>(lua, 1);
            coll.isTrigger = lua.ToBoolean(2);
            return 0;
        }

        private static int GetBounds(ILuaState lua) {
            Collider coll = Utils.CheckUnityObject<Collider>(lua, 1);
            lua.PushLightUserData(new UnityBounds(coll.bounds));
            return 1;
        }
    }
}