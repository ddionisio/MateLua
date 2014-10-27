﻿using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class UnityCollider {
        public const string META_NAME = "Unity.Collider.Meta";
        public const string LIB_NAME = "Unity.Collider";

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

                    new NameFuncPair("closestPointOnBounds", ClosestPointOnBounds),
                    new NameFuncPair("raycast", Raycast),
                };
            if(l_funcs == null)
                l_funcs = new NameFuncPair[] {
                    new NameFuncPair("Cast", Cast),
                };

            Utils.NewLibMetaGetterSetter(lua, META_NAME, m_funcs, l_funcs);

            return 1;
        }

        public static void Push(ILuaState lua, Collider coll) {
            if(coll) {
                lua.NewUserData(coll);
                lua.SetMetaTable(META_NAME);
            }
            else //null given,
                lua.PushNil();
        }

        private static int Cast(ILuaState lua) {
            lua.NewUserData(Utils.CheckUnityObject<Collider>(lua, 1));
            lua.SetMetaTable(META_NAME);
            return 1;
        }

        private static int Get(ILuaState lua) {
            Collider coll = Utils.CheckUnityObject<Collider>(lua, 1);
            string field = lua.L_CheckString(2);
            switch(field) {
                case "enabled":
                    lua.PushBoolean(coll.enabled);
                    break;
                case "isTrigger":
                    lua.PushBoolean(coll.isTrigger);
                    break;
                case "bounds":
                    lua.NewUserData(new UnityBounds(coll.bounds));
                    lua.SetMetaTable(UnityBounds.META_NAME);
                    break;
                case "attachedRigidbody":
                    UnityRigidbody.Push(lua, coll.attachedRigidbody);
                    break;
                default:
                    if(!UnityComponent.PushField(lua, coll, field))
                        if(!lua.L_GetMetaField(1, field))
                            lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 1;
        }

        private static int Set(ILuaState lua) {
            Collider coll = Utils.CheckUnityObject<Collider>(lua, 1);
            string field = lua.L_CheckString(2);
            switch(field) {
                case "enabled":
                    coll.enabled = lua.ToBoolean(3);
                    break;
                case "isTrigger":
                    coll.isTrigger = lua.ToBoolean(3);
                    break;
                default:
                    if(!UnityComponent.SetField(lua, coll, field))
                        lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 0;
        }

        private static int ClosestPointOnBounds(ILuaState lua) {
            Collider coll = Utils.CheckUnityObject<Collider>(lua, 1);
            Vector3 pt = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            Vector3 ret = coll.ClosestPointOnBounds(pt);
            lua.PushNumber(ret.x);
            lua.PushNumber(ret.y);
            lua.PushNumber(ret.z);
            return 3;
        }

        private static int Raycast(ILuaState lua) {
            lua.L_Error("Not Implemented");
            //Collider coll = Utils.CheckUnityObject<Collider>(lua, 1);
            //TODO: need RaycastHit
            return 0;
        }
    }
}