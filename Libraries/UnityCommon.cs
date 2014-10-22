using UnityEngine;

using UniLua;

namespace M8.Lua.Library {
	public static class LGameObject {
        public const string LIB_NAME = "Unity.GameObject";

        public static int OpenLib(ILuaState lua) {
            NameFuncPair[] funcs = new NameFuncPair[] {
                new NameFuncPair("isActiveSelf", IsActiveSelf),
                new NameFuncPair("isActiveInHierarchy", IsActiveInHierarchy),
                new NameFuncPair("setActive", SetActive),
                new NameFuncPair("getName", GetName),
                new NameFuncPair("getLayer", GetLayer),
                new NameFuncPair("getTag", GetTag),
                new NameFuncPair("compareTag", CompareTag),
                new NameFuncPair("getComponent", GetComponent),
                new NameFuncPair("getComponentInChildren", GetComponentInChildren),
                new NameFuncPair("getComponentInParent", GetComponentInParent),
                new NameFuncPair("getTransform", GetTransform),
                new NameFuncPair("getCollider", GetCollider),
                new NameFuncPair("getRigidbody", GetRigidbody),
                new NameFuncPair("sendMessage", SendMessage),
                new NameFuncPair("sendMessageUpwards", SendMessageUpwards),
                new NameFuncPair("broadcastMessage", BroadcastMessage),
            };

            lua.L_NewLib(funcs);
            return 1;
        }

        //Note: expect first param to be light object reference to GameObject

        public static GameObject CheckGameObject(ILuaState lua, int ind) {
            GameObject go = lua.ToUserData(ind) as GameObject;
            if(go == null)
                lua.L_ArgError(ind, "Not a GameObject");
            return go;
        }

        /// <summary>
        /// Returns if given GameObject is active. bool IsActiveSelf(go)
        /// </summary>
        /// <returns>string</returns>
        private static int IsActiveSelf(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
            lua.PushBoolean(go.activeSelf);
            return 1;
        }

        /// <summary>
        /// Returns if given GameObject is active in hierarchy. bool IsActiveInHierarchy(go)
        /// </summary>
        /// <returns>string</returns>
        private static int IsActiveInHierarchy(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
            lua.PushBoolean(go.activeInHierarchy);
            return 1;
        }

        /// <summary>
        /// set given GameObject active. SetActive(go, bool)
        /// </summary>
        private static int SetActive(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
            bool active = lua.ToBoolean(2);
            go.SetActive(active);
            return 1;
        }

        /// <summary>
        /// Returns name of given GameObject. string GetName(go)
        /// </summary>
        /// <returns>string</returns>
        private static int GetName(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
            lua.PushString(go.name);
            return 1;
        }

        /// <summary>
        /// Returns the layer of GameObject. int GetLayer(go)
        /// </summary>
        /// <returns>layer index [0, 31]</returns>
        private static int GetLayer(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
            lua.PushInteger(go.layer);
            return 1;
        }

        /// <summary>
        /// Returns the tag of GameObject. string GetTag(go)
        /// </summary>
        /// <returns>tag</returns>
        private static int GetTag(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
            lua.PushString(go.tag);
            return 1;
        }

        /// <summary>
        /// Returns if tag is true. bool CompareTag(go, tag)
        /// </summary>
        /// <returns>bool</returns>
        private static int CompareTag(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
            string tag = lua.L_CheckString(2);
            lua.PushBoolean(go.CompareTag(tag));
            return 1;
        }

        /// <summary>
        /// Returns the component of GameObject based on type name. Component GetComponent(go, type)
        /// </summary>
        /// <returns>component</returns>
        private static int GetComponent(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
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
            GameObject go = CheckGameObject(lua, 1);
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
            GameObject go = CheckGameObject(lua, 1);
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
            GameObject go = CheckGameObject(lua, 1);
            lua.PushLightUserData(go.transform);
            return 1;
        }

        /// <summary>
        /// Returns the collider of GameObject. collider GetCollider(go)
        /// </summary>
        /// <returns>transform</returns>
        private static int GetCollider(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
            Collider coll = go.collider;
            if(coll)
                lua.PushLightUserData(go.collider);
            else
                lua.PushNil();
            return 1;
        }

        /// <summary>
        /// Returns the rigidbody of GameObject. rigidbody GetRigidbody(go)
        /// </summary>
        /// <returns>transform</returns>
        private static int GetRigidbody(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
            Rigidbody body = go.rigidbody;
            if(body)
                lua.PushLightUserData(go.rigidbody);
            else
                lua.PushNil();
            return 1;
        }

        /// <summary>
        /// Send message. SendMessage(go, method, var)
        /// </summary>
        private static int SendMessage(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
            string method = lua.L_CheckString(2);
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
            return 1;
        }

        /// <summary>
        /// Send message. SendMessageUpwards(go, method, var)
        /// </summary>
        private static int SendMessageUpwards(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
            string method = lua.L_CheckString(2);
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
            return 1;
        }

        /// <summary>
        /// Send message. BroadcastMessage(go, method, var)
        /// </summary>
        private static int BroadcastMessage(ILuaState lua) {
            GameObject go = CheckGameObject(lua, 1);
            string method = lua.L_CheckString(2);
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
            return 1;
        }
	}
}