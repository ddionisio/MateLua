using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class UnityCommon {
        public static void PushGameObject(ILuaState lua, GameObject go) {
            if(go) {
                lua.PushLightUserData(go);
                lua.SetMetaTable(UnityGameObject.META_NAME);
            }
            else
                lua.PushNil();
        }

        public static void PushComponent(ILuaState lua, Component c) {
            if(c) {
                lua.PushLightUserData(c);

                //determine meta table
                if(c is Transform)
                    lua.SetMetaTable(UnityTransform.META_NAME);
                else if(c is Behaviour) //default to Behaviour
                    lua.SetMetaTable(UnityBehaviour.META_NAME);
                else //default to Component
                    lua.SetMetaTable(UnityComponent.META_NAME);
            }
            else //null given,
                lua.PushNil();
        }
    }

#region Object
    public static class UnityObject {
        public const string META_NAME = "Unity.Object.Meta";
        public const string LIB_NAME = "Unity.Object";

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
                    new NameFuncPair("Cast", Cast),
                    new NameFuncPair("Instantiate", Instantiate),
                };

            Utils.NewLibMetaGetterSetter(lua, META_NAME, m_funcs, l_funcs);

            return 1;
        }

        private static int Cast(ILuaState lua) {
            Object o = Utils.CheckUnityObject<Object>(lua, 1);
            lua.PushLightUserData(o);
            lua.SetMetaTable(META_NAME);
            return 1;
        }

        private static int Instantiate(ILuaState lua) {
            Object o = Utils.CheckUnityObject<Object>(lua, 1);
            Object newObj = Object.Instantiate(o);
            if(newObj is GameObject) {
                //return with meta
                lua.PushLightUserData(newObj);
            }
            else {
                lua.PushLightUserData(newObj);
                lua.SetMetaTable(META_NAME);
            }
            return 1;
        }

        private static int Get(ILuaState lua) {
            Object o = Utils.CheckUnityObject<Object>(lua, 1);
            string field = lua.L_CheckString(2);
            switch(field) {
                case "name":
                    lua.PushString(o.name);
                    break;
                default:
                    if(!lua.L_GetMetaField(1, field))
                        lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 1;
        }

        private static int Set(ILuaState lua) {
            Object o = Utils.CheckUnityObject<Object>(lua, 1);
            string field = lua.L_CheckString(2);
            switch(field) {
                case "name":
                    o.name = lua.L_CheckString(3);
                    break;
                default:
                    lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 0;
        }
    }
#endregion

#region GameObject
    public static class UnityGameObject {
        public const string META_NAME = "Unity.GameObject.Meta";
        public const string LIB_NAME = "Unity.GameObject";

        private static NameFuncPair[] m_funcs = null;
        private static NameFuncPair[] l_funcs = null;

        public static int OpenLib(ILuaState lua) {
            if(m_funcs == null)
                m_funcs = new NameFuncPair[] {
                    new NameFuncPair(Utils.GETTER, Get),
                    new NameFuncPair(Utils.SETTER, Set),

                    new NameFuncPair("setActive", SetActive),

                    new NameFuncPair("compareTag", CompareTag),
                    new NameFuncPair("getComponent", GetComponent),
                    new NameFuncPair("getComponentInChildren", GetComponentInChildren),
                    new NameFuncPair("getComponentInParent", GetComponentInParent),
                    new NameFuncPair("sendMessage", SendMessage),
                    new NameFuncPair("sendMessageUpwards", SendMessageUpwards),
                    new NameFuncPair("broadcastMessage", BroadcastMessage),
                };
            if(l_funcs == null)
                l_funcs = new NameFuncPair[] {
                    new NameFuncPair("New", New),
                    new NameFuncPair("Find", Find),
                    new NameFuncPair("FindGameObjectWithTag", FindGameObjectWithTag),
                };

            Utils.NewLibMetaGetterSetter(lua, META_NAME, m_funcs, l_funcs);

            return 1;
        }

        private static int Get(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            string field = lua.L_CheckString(2);
            
            switch(field) {
                case "name":
                    lua.PushString(go.name);
                    break;
                case "activeSelf":
                    lua.PushBoolean(go.activeSelf);
                    break;
                case "activeInHierarchy":
                    lua.PushBoolean(go.activeInHierarchy);
                    break;
                case "layer":
                    lua.PushInteger(go.layer);
                    break;
                case "tag":
                    lua.PushString(go.tag);
                    break;
                case "transform":
                    UnityCommon.PushComponent(lua, go.transform);
                    break;
                case "collider":
                    UnityCommon.PushComponent(lua, go.collider);
                    break;
                case "rigidbody":
                    UnityCommon.PushComponent(lua, go.rigidbody);
                    break;
                default:
                    if(!lua.L_GetMetaField(1, field))
                        lua.L_Error("Unknown field: {0}", field);
                    break;
            }

            return 1;
        }

        private static int Set(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            string field = lua.L_CheckString(2);

            switch(field) {
                case "name":
                    go.name = lua.L_CheckString(3);
                    break;
                case "layer":
                    go.layer = lua.L_CheckInteger(3);
                    break;
                case "tag":
                    go.tag = lua.L_CheckString(3);
                    break;
                default:
                    lua.L_Error("Unknown field: {0}", field);
                    break;
            }
                
            return 0;
        }

        private static int New(ILuaState lua) {
            string name = lua.L_CheckString(1);

            //components?

            GameObject go = new GameObject(name);
            UnityCommon.PushGameObject(lua, go);
            return 1;
        }

        private static int Find(ILuaState lua) {
            string name = lua.L_CheckString(1);

            GameObject go = GameObject.Find(name);
            UnityCommon.PushGameObject(lua, go);
            return 1;
        }

        private static int FindGameObjectWithTag(ILuaState lua) {
            string tag = lua.L_CheckString(1);

            GameObject go = GameObject.FindGameObjectWithTag(tag);
            UnityCommon.PushGameObject(lua, go);
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
            UnityCommon.PushComponent(lua, c);
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
                UnityCommon.PushComponent(lua, c);
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
                UnityCommon.PushComponent(lua, c);
            }
            else
                lua.L_ArgError(2, "Type not exists: "+typeString);
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
        public const string META_NAME = "Unity.Component.Meta";
        public const string LIB_NAME = "Unity.Component";

        private static NameFuncPair[] m_funcs = null;
        private static NameFuncPair[] l_funcs = null;

        public static int OpenLib(ILuaState lua) {
            if(m_funcs == null)
                m_funcs = new NameFuncPair[] {
                    new NameFuncPair(Utils.GETTER, Get),
                    new NameFuncPair(Utils.SETTER, Set),

                    new NameFuncPair("compareTag", CompareTag),
                    new NameFuncPair("getComponent", GetComponent),
                    new NameFuncPair("getComponentInChildren", GetComponentInChildren),
                    new NameFuncPair("getComponentInParent", GetComponentInParent),

                    new NameFuncPair("sendMessage", SendMessage),
                    new NameFuncPair("sendMessageUpwards", SendMessageUpwards),
                    new NameFuncPair("broadcastMessage", BroadcastMessage),
                };
            if(l_funcs == null)
                l_funcs = new NameFuncPair[] {
                };

            Utils.NewLibMetaGetterSetter(lua, META_NAME, m_funcs, l_funcs);

            return 1;
        }

        public static bool PushField(ILuaState lua, Component comp, string field) {
            switch(field) {
                case "name":
                    lua.PushString(comp.name);
                    return true;
                case "tag":
                    lua.PushString(comp.tag);
                    return true;
                case "gameObject":
                    UnityCommon.PushGameObject(lua, comp.gameObject);
                    return true;
                case "transform":
                    UnityCommon.PushComponent(lua, comp.transform);
                    return true;
                case "collider":
                    UnityCommon.PushComponent(lua, comp.collider);
                    return true;
                case "rigidbody":
                    UnityCommon.PushComponent(lua, comp.rigidbody);
                    return true;
                default:
                    return false;
            }
        }
                
        public static bool SetField(ILuaState lua, Component comp, string field) {
            switch(field) {
                case "name":
                    comp.name = lua.L_CheckString(3);
                    return true;
                case "tag":
                    comp.tag = lua.L_CheckString(3);
                    return true;
                default:
                    return false;
            }
        }

        private static int Get(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string field = lua.L_CheckString(2);
            if(!PushField(lua, comp, field))
                if(!lua.L_GetMetaField(1, field))
                    lua.L_Error("Unknown field: {0}", field);
            return 1;
        }

        private static int Set(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string field = lua.L_CheckString(2);
            if(!SetField(lua, comp, field))
                lua.L_Error("Unknown field: {0}", field);
            return 0;
        }

        public static int CompareTag(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string tag = lua.L_CheckString(2);
            lua.PushBoolean(comp.CompareTag(tag));
            return 1;
        }

        public static int GetComponent(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string type = lua.L_CheckString(2);

            Component c = comp.GetComponent(type);
            UnityCommon.PushComponent(lua, c);
            return 1;
        }

        public static int GetComponentInChildren(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string typeString = lua.L_CheckString(2);
            System.Type type = System.Type.GetType(typeString);
            if(type != null) {
                Component c = comp.GetComponentInChildren(type);
                UnityCommon.PushComponent(lua, c);
            }
            else
                lua.L_ArgError(2, "Type not exists: "+typeString);
            return 1;
        }

        public static int GetComponentInParent(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            string typeString = lua.L_CheckString(2);
            System.Type type = System.Type.GetType(typeString);
            if(type != null) {
                Component c = comp.GetComponentInParent(type);
                UnityCommon.PushComponent(lua, c);
            }
            else
                lua.L_ArgError(2, "Type not exists: "+typeString);
            return 1;
        }

        public static int SendMessage(ILuaState lua) {
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

        public static int SendMessageUpwards(ILuaState lua) {
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
        public static int BroadcastMessage(ILuaState lua) {
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
        public const string META_NAME = "Unity.Behaviour.Meta";
        public const string LIB_NAME = "Unity.Behaviour";

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
                };

            Utils.NewLibMetaGetterSetter(lua, META_NAME, m_funcs, l_funcs);

            return 1;
        }

        public static bool PushField(ILuaState lua, Behaviour b, string field) {
            if(!UnityComponent.PushField(lua, b, field)) {
                switch(field) {
                    case "enabled":
                        lua.PushBoolean(b.enabled);
                        return true;
                }
            }
            return false;
        }
                
        public static bool SetField(ILuaState lua, Behaviour b, string field) {
            if(!UnityComponent.SetField(lua, b, field)) {
                switch(field) {
                    case "enabled":
                        b.enabled = lua.ToBoolean(3);
                        return true;
                }
            }
            return false;
        }

        private static int Get(ILuaState lua) {
            Behaviour b = Utils.CheckUnityObject<Behaviour>(lua, 1);
            string field = lua.L_CheckString(2);
            if(!PushField(lua, b, field))
                if(!lua.L_GetMetaField(1, field))
                    lua.L_Error("Unknown field: {0}", field);
            return 1;
        }

        private static int Set(ILuaState lua) {
            Behaviour b = Utils.CheckUnityObject<Behaviour>(lua, 1);
            string field = lua.L_CheckString(2);
            if(!SetField(lua, b, field))
                lua.L_Error("Unknown field: {0}", field);
            return 0;
        }
    }
#endregion
}