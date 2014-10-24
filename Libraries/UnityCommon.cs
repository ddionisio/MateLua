using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
#region Object
    public static class UnityObject {
        public const string LIB_NAME = "Unity.Object";

        public static int OpenLib(ILuaState lua) {
            NameFuncPair[] funcs = new NameFuncPair[] {
                //object
                new NameFuncPair("GetName", GetName),
                new NameFuncPair("SetName", SetName),
            };

            lua.L_NewLib(funcs);
            return 1;
        }

        /// <summary>
        /// Returns name of given Object. string GetName(obj)
        /// </summary>
        /// <returns>string</returns>
        private static int GetName(ILuaState lua) {
            Object o = Utils.CheckUnityObject<Object>(lua, 1);
            lua.PushString(o.name);
            return 1;
        }

        /// <summary>
        /// set given Object name. SetName(obj, string)
        /// </summary>
        private static int SetName(ILuaState lua) {
            Object o = Utils.CheckUnityObject<Object>(lua, 1);
            o.name = lua.L_CheckString(2);
            return 0;
        }
    }
#endregion

#region GameObject
    public static class UnityGameObject {
        public const string LIB_NAME = "Unity.GameObject";

        public static int OpenLib(ILuaState lua) {
            NameFuncPair[] funcs = new NameFuncPair[] {
                new NameFuncPair("IsActiveSelf", IsActiveSelf),
                new NameFuncPair("IsActiveInHierarchy", IsActiveInHierarchy),
                new NameFuncPair("SetActive", SetActive),
                new NameFuncPair("GetLayer", GetLayer),
                new NameFuncPair("SetLayer", SetLayer),

                new NameFuncPair("GetTag", GetTag),
                new NameFuncPair("SetTag", SetTag),
                new NameFuncPair("CompareTag", CompareTag),
                new NameFuncPair("GetComponent", GetComponent),
                new NameFuncPair("GetComponentInChildren", GetComponentInChildren),
                new NameFuncPair("GetComponentInParent", GetComponentInParent),
                new NameFuncPair("GetTransform", GetTransform),
                new NameFuncPair("GetCollider", GetCollider),
                new NameFuncPair("GetRigidbody", GetRigidbody),
                new NameFuncPair("SendMessage", SendMessage),
                new NameFuncPair("SendMessageUpwards", SendMessageUpwards),
                new NameFuncPair("BroadcastMessage", BroadcastMessage),
            };

            lua.L_NewLib(funcs);
            return 1;
        }

        /// <summary>
        /// Returns if given GameObject is active. bool IsActiveSelf(go)
        /// </summary>
        /// <returns>string</returns>
        private static int IsActiveSelf(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            lua.PushBoolean(go.activeSelf);
            return 1;
        }

        /// <summary>
        /// Returns if given GameObject is active in hierarchy. bool IsActiveInHierarchy(go)
        /// </summary>
        /// <returns>string</returns>
        private static int IsActiveInHierarchy(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            lua.PushBoolean(go.activeInHierarchy);
            return 1;
        }

        /// <summary>
        /// set given GameObject active. SetActive(go, bool)
        /// </summary>
        private static int SetActive(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            bool active = lua.ToBoolean(2);
            go.SetActive(active);
            return 0;
        }

        /// <summary>
        /// Returns the layer of GameObject. int GetLayer(go)
        /// </summary>
        /// <returns>layer index [0, 31]</returns>
        private static int GetLayer(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            lua.PushInteger(go.layer);
            return 1;
        }

        /// <summary>
        /// set given GameObject layer. SetTag(go, int)
        /// </summary>
        private static int SetLayer(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            go.layer = lua.L_CheckInteger(2);
            return 0;
        }

        /// <summary>
        /// Returns the tag of GameObject. string GetTag(go)
        /// </summary>
        /// <returns>tag</returns>
        private static int GetTag(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            lua.PushString(go.tag);
            return 1;
        }

        /// <summary>
        /// set given GameObject tag. SetTag(go, string)
        /// </summary>
        private static int SetTag(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            go.tag = lua.L_CheckString(2);
            return 0;
        }

        /// <summary>
        /// Returns if tag is true. bool CompareTag(go, tag)
        /// </summary>
        /// <returns>bool</returns>
        private static int CompareTag(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            string tag = lua.L_CheckString(2);
            lua.PushBoolean(go.CompareTag(tag));
            return 1;
        }

        /// <summary>
        /// Returns the component of GameObject based on type name. Component GetComponent(go, type)
        /// </summary>
        /// <returns>component</returns>
        private static int GetComponent(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            string type = lua.L_CheckString(2);

            Component c = go.GetComponent(type);
            if(c)
                lua.PushLightUserData(c);
            else
                lua.PushNil();
            return 1;
        }

        /// <summary>
        /// Returns the component of GameObject based on type name. Component GetComponentInChildren(go, type)
        /// </summary>
        /// <returns>component</returns>
        private static int GetComponentInChildren(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            string typeString = lua.L_CheckString(2);
            System.Type type = System.Type.GetType(typeString);
            if(type != null) {
                Component c = go.GetComponentInChildren(type);
                if(c)
                    lua.PushLightUserData(c);
                else
                    lua.PushNil();
            }
            else
                lua.L_ArgError(2, "Type not exists: "+typeString);
            return 1;
        }

        /// <summary>
        /// Returns the component of GameObject based on type name. Component GetComponentInParent(go, type)
        /// </summary>
        /// <returns>component</returns>
        private static int GetComponentInParent(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            string typeString = lua.L_CheckString(2);
            System.Type type = System.Type.GetType(typeString);
            if(type != null) {
                Component c = go.GetComponentInParent(type);
                if(c)
                    lua.PushLightUserData(c);
                else
                    lua.PushNil();
            }
            else
                lua.L_ArgError(2, "Type not exists: "+typeString);
            return 1;
        }

        /// <summary>
        /// Returns the transform of GameObject. Transform GetTransform(go)
        /// </summary>
        /// <returns>transform</returns>
        private static int GetTransform(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            lua.PushLightUserData(go.transform);
            return 1;
        }

        /// <summary>
        /// Returns the collider of GameObject. collider GetCollider(go)
        /// </summary>
        /// <returns>transform</returns>
        private static int GetCollider(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            Collider coll = go.collider;
            if(coll)
                lua.PushLightUserData(coll);
            else
                lua.PushNil();
            return 1;
        }

        /// <summary>
        /// Returns the rigidbody of GameObject. rigidbody GetRigidbody(go)
        /// </summary>
        /// <returns>transform</returns>
        private static int GetRigidbody(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            Rigidbody body = go.rigidbody;
            if(body)
                lua.PushLightUserData(body);
            else
                lua.PushNil();
            return 1;
        }

        /// <summary>
        /// Send message. SendMessage(go, method, var)
        /// </summary>
        private static int SendMessage(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            string method = lua.L_CheckString(2);

            if(lua.GetTop() >= 3) {
                switch(lua.Type(3)) {
                    case LuaType.LUA_TBOOLEAN:
                        go.SendMessage(method, lua.ToBoolean(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TNUMBER:
                        go.SendMessage(method, ((float)lua.ToNumber(3)), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TSTRING:
                        go.SendMessage(method, lua.ToString(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TUINT64:
                        go.SendMessage(method, lua.ToInteger(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TLIGHTUSERDATA:
                        go.SendMessage(method, lua.ToUserData(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    default:
                        lua.L_ArgError(3, "Incompatible Type");
                        break;
                }
            }
            else
                go.SendMessage(method, null, SendMessageOptions.DontRequireReceiver);
            return 0;
        }

        /// <summary>
        /// Send message. SendMessageUpwards(go, method, var)
        /// </summary>
        private static int SendMessageUpwards(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            string method = lua.L_CheckString(2);

            if(lua.GetTop() >= 3) {
                switch(lua.Type(3)) {
                    case LuaType.LUA_TBOOLEAN:
                        go.SendMessageUpwards(method, lua.ToBoolean(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TNUMBER:
                        go.SendMessageUpwards(method, ((float)lua.ToNumber(3)), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TSTRING:
                        go.SendMessageUpwards(method, lua.ToString(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TUINT64:
                        go.SendMessageUpwards(method, lua.ToInteger(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TLIGHTUSERDATA:
                        go.SendMessageUpwards(method, lua.ToUserData(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    default:
                        lua.L_ArgError(3, "Incompatible Type");
                        break;
                }
            }
            else
                go.SendMessageUpwards(method, null, SendMessageOptions.DontRequireReceiver);
            return 0;
        }

        /// <summary>
        /// Send message. BroadcastMessage(go, method, var)
        /// </summary>
        private static int BroadcastMessage(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            string method = lua.L_CheckString(2);

            if(lua.GetTop() >= 3) {
                switch(lua.Type(3)) {
                    case LuaType.LUA_TBOOLEAN:
                        go.BroadcastMessage(method, lua.ToBoolean(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TNUMBER:
                        go.BroadcastMessage(method, ((float)lua.ToNumber(3)), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TSTRING:
                        go.BroadcastMessage(method, lua.ToString(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TUINT64:
                        go.BroadcastMessage(method, lua.ToInteger(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TLIGHTUSERDATA:
                        go.BroadcastMessage(method, lua.ToUserData(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    default:
                        lua.L_ArgError(3, "Incompatible Type");
                        break;
                }
            }
            else
                go.BroadcastMessage(method, null, SendMessageOptions.DontRequireReceiver);
            return 0;
        }
    }
#endregion

#region Component
    public static class UnityComponent {
        public const string LIB_NAME = "Unity.Component";

        public static int OpenLib(ILuaState lua) {
            NameFuncPair[] funcs = new NameFuncPair[] {
                new NameFuncPair("GetGameObject", GetGameObject),

                new NameFuncPair("GetTag", GetTag),
                new NameFuncPair("SetTag", SetTag),
                new NameFuncPair("CompareTag", CompareTag),
                new NameFuncPair("GetComponent", GetComponent),
                new NameFuncPair("GetComponentInChildren", GetComponentInChildren),
                new NameFuncPair("GetComponentInParent", GetComponentInParent),
                new NameFuncPair("GetTransform", GetTransform),
                new NameFuncPair("GetCollider", GetCollider),
                new NameFuncPair("GetRigidbody", GetRigidbody),
                new NameFuncPair("SendMessage", SendMessage),
                new NameFuncPair("SendMessageUpwards", SendMessageUpwards),
                new NameFuncPair("BroadcastMessage", BroadcastMessage),
            };

            lua.L_NewLib(funcs);
            return 1;
        }

        private static int GetGameObject(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            lua.PushLightUserData(comp.gameObject);
            return 1;
        }

        private static int GetTag(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            lua.PushString(comp.tag);
            return 1;
        }

        private static int SetTag(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            comp.tag = lua.L_CheckString(2);
            return 0;
        }

        private static int CompareTag(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string tag = lua.L_CheckString(2);
            lua.PushBoolean(comp.CompareTag(tag));
            return 1;
        }

        private static int GetComponent(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string type = lua.L_CheckString(2);

            Component c = comp.GetComponent(type);
            if(c)
                lua.PushLightUserData(c);
            else
                lua.PushNil();
            return 1;
        }

        private static int GetComponentInChildren(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string typeString = lua.L_CheckString(2);
            System.Type type = System.Type.GetType(typeString);
            if(type != null) {
                Component c = comp.GetComponentInChildren(type);
                if(c)
                    lua.PushLightUserData(c);
                else
                    lua.PushNil();
            }
            else
                lua.L_ArgError(2, "Type not exists: "+typeString);
            return 1;
        }

        private static int GetComponentInParent(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string typeString = lua.L_CheckString(2);
            System.Type type = System.Type.GetType(typeString);
            if(type != null) {
                Component c = comp.GetComponentInParent(type);
                if(c)
                    lua.PushLightUserData(c);
                else
                    lua.PushNil();
            }
            else
                lua.L_ArgError(2, "Type not exists: "+typeString);
            return 1;
        }

        private static int GetTransform(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            lua.PushLightUserData(comp.transform);
            return 1;
        }

        private static int GetCollider(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            Collider coll = comp.collider;
            if(coll)
                lua.PushLightUserData(coll);
            else
                lua.PushNil();
            return 1;
        }

        private static int GetRigidbody(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            Rigidbody body = comp.rigidbody;
            if(body)
                lua.PushLightUserData(body);
            else
                lua.PushNil();
            return 1;
        }

        private static int SendMessage(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string method = lua.L_CheckString(2);

            if(lua.GetTop() >= 3) {
                switch(lua.Type(3)) {
                    case LuaType.LUA_TBOOLEAN:
                        comp.SendMessage(method, lua.ToBoolean(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TNUMBER:
                        comp.SendMessage(method, ((float)lua.ToNumber(3)), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TSTRING:
                        comp.SendMessage(method, lua.ToString(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TUINT64:
                        comp.SendMessage(method, lua.ToInteger(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TLIGHTUSERDATA:
                        comp.SendMessage(method, lua.ToUserData(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    default:
                        lua.L_ArgError(3, "Incompatible Type");
                        break;
                }
            }
            else
                comp.SendMessage(method, null, SendMessageOptions.DontRequireReceiver);
            return 0;
        }

        private static int SendMessageUpwards(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string method = lua.L_CheckString(2);

            if(lua.GetTop() >= 3) {
                switch(lua.Type(3)) {
                    case LuaType.LUA_TBOOLEAN:
                        comp.SendMessageUpwards(method, lua.ToBoolean(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TNUMBER:
                        comp.SendMessageUpwards(method, ((float)lua.ToNumber(3)), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TSTRING:
                        comp.SendMessageUpwards(method, lua.ToString(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TUINT64:
                        comp.SendMessageUpwards(method, lua.ToInteger(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TLIGHTUSERDATA:
                        comp.SendMessageUpwards(method, lua.ToUserData(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    default:
                        lua.L_ArgError(3, "Incompatible Type");
                        break;
                }
            }
            else
                comp.SendMessageUpwards(method, null, SendMessageOptions.DontRequireReceiver);
            return 0;
        }

        /// <summary>
        /// Send message. BroadcastMessage(go, method, var)
        /// </summary>
        private static int BroadcastMessage(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string method = lua.L_CheckString(2);

            if(lua.GetTop() >= 3) {
                switch(lua.Type(3)) {
                    case LuaType.LUA_TBOOLEAN:
                        comp.BroadcastMessage(method, lua.ToBoolean(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TNUMBER:
                        comp.BroadcastMessage(method, ((float)lua.ToNumber(3)), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TSTRING:
                        comp.BroadcastMessage(method, lua.ToString(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TUINT64:
                        comp.BroadcastMessage(method, lua.ToInteger(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TLIGHTUSERDATA:
                        comp.BroadcastMessage(method, lua.ToUserData(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    default:
                        lua.L_ArgError(3, "Incompatible Type");
                        break;
                }
            }
            else
                comp.BroadcastMessage(method, null, SendMessageOptions.DontRequireReceiver);
            return 0;
        }
    }
#endregion

#region Behaviour
    public static class UnityBehaviour {
        public const string LIB_NAME = "Unity.Behaviour";

        public static int OpenLib(ILuaState lua) {
            NameFuncPair[] funcs = new NameFuncPair[] {
                new NameFuncPair("IsEnabled", IsEnabled),
                new NameFuncPair("SetEnabled", SetEnabled),
            };

            lua.L_NewLib(funcs);
            return 1;
        }

        /// <summary>
        /// Check if given Behaviour is enabled. bool IsEnabled(behaviour)
        /// </summary>
        private static int IsEnabled(ILuaState lua) {
            Behaviour b = Utils.CheckUnityObject<Behaviour>(lua, 1);
            lua.PushBoolean(b.enabled);
            return 1;
        }

        /// <summary>
        /// Set Behaviour enabled. SetEnabled(behaviour)
        /// </summary>
        private static int SetEnabled(ILuaState lua) {
            Behaviour b = Utils.CheckUnityObject<Behaviour>(lua, 1);
            b.enabled = lua.ToBoolean(2);
            return 0;
        }
    }
#endregion
}