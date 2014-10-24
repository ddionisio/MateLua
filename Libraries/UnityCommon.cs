using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
#region Common
    public static class UnityCommon {
        public const string LIB_NAME = "Unity.Common";

        public static int OpenLib(ILuaState lua) {
            NameFuncPair[] funcs = new NameFuncPair[] {
                //object
                new NameFuncPair("GetName", GetName),
                new NameFuncPair("SetName", SetName),

                //gameobject
                new NameFuncPair("IsActiveSelf", IsActiveSelf),
                new NameFuncPair("IsActiveInHierarchy", IsActiveInHierarchy),
                new NameFuncPair("SetActive", SetActive),
                new NameFuncPair("GetLayer", GetLayer),
                new NameFuncPair("SetLayer", SetLayer),
                
                //component
                new NameFuncPair("GetGameObject", GetGameObject),
                
                //behaviour
                new NameFuncPair("IsEnabled", IsEnabled),
                new NameFuncPair("SetEnabled", SetEnabled),

                //gameobject/component
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

        //Note: expect first param to be light object reference to Object

    #region Object
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
    #endregion

    #region GameObject
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
    #endregion

    #region Component
        /// <summary>
        /// get gameobject of component.  GameObject GetGameObject(comp)
        /// </summary>
        private static int GetGameObject(ILuaState lua) {
            Component comp = Utils.CheckUnityObject<Component>(lua, 1);
            lua.PushLightUserData(comp.gameObject);
            return 1;
        }
    #endregion

    #region Behaviour
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
    #endregion

    #region GameObject_Component
        /// <summary>
        /// Returns the tag of GameObject. string GetTag(go)
        /// </summary>
        /// <returns>tag</returns>
        private static int GetTag(ILuaState lua) {
            object o = lua.ToUserData(1);

            GameObject go = o as GameObject;
            if(go)
                lua.PushString(go.tag);
            else {
                Component comp = o as Component;
                if(comp)
                    lua.PushString(comp.tag);
                else
                    lua.L_ArgError(1, "Not a GameObject or Component");
            }
            return 1;
        }

        /// <summary>
        /// set given GameObject tag. SetTag(go, string)
        /// </summary>
        private static int SetTag(ILuaState lua) {
            object o = lua.ToUserData(1);

            GameObject go = o as GameObject;
            if(go)
                go.tag = lua.L_CheckString(2);
            else {
                Component comp = o as Component;
                if(comp)
                    comp.tag = lua.L_CheckString(2);
                else
                    lua.L_ArgError(1, "Not a GameObject or Component");
            }
            return 0;
        }

        /// <summary>
        /// Returns if tag is true. bool CompareTag(go, tag)
        /// </summary>
        /// <returns>bool</returns>
        private static int CompareTag(ILuaState lua) {
            object o = lua.ToUserData(1);
            string tag = lua.L_CheckString(2);

            GameObject go = o as GameObject;
            if(go)
                lua.PushBoolean(go.CompareTag(tag));
            else {
                Component comp = o as Component;
                if(comp)
                    lua.PushBoolean(comp.CompareTag(tag));
                else
                    lua.L_ArgError(1, "Not a GameObject or Component");
            }
            return 1;
        }

        /// <summary>
        /// Returns the component of GameObject based on type name. Component GetComponent(go, type)
        /// </summary>
        /// <returns>component</returns>
        private static int GetComponent(ILuaState lua) {
            object o = lua.ToUserData(1);
            string type = lua.L_CheckString(2);

            Component c = null;

            GameObject go = o as GameObject;
            if(go)
                c = go.GetComponent(type);
            else {
                Component comp = o as Component;
                if(comp)
                    c = comp.GetComponent(type);
                else
                    lua.L_ArgError(1, "Not a GameObject or Component");
            }

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
            string typeString = lua.L_CheckString(2);
            System.Type type = System.Type.GetType(typeString);
            if(type != null) {
                object o = lua.ToUserData(1);

                Component c = null;

                GameObject go = o as GameObject;
                if(go)
                    c = go.GetComponentInChildren(type);
                else {
                    Component comp = o as Component;
                    if(comp)
                        c = comp.GetComponentInChildren(type);
                    else
                        lua.L_ArgError(1, "Not a GameObject or Component");
                }

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
            string typeString = lua.L_CheckString(2);
            System.Type type = System.Type.GetType(typeString);
            if(type != null) {
                object o = lua.ToUserData(1);

                Component c = null;

                GameObject go = o as GameObject;
                if(go)
                    c = go.GetComponentInParent(type);
                else {
                    Component comp = o as Component;
                    if(comp)
                        c = comp.GetComponentInParent(type);
                    else
                        lua.L_ArgError(1, "Not a GameObject or Component");
                }

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
            object o = lua.ToUserData(1);

            GameObject go = o as GameObject;
            if(go)
                lua.PushLightUserData(go.transform);
            else {
                Component comp = o as Component;
                if(comp)
                    lua.PushLightUserData(comp.transform);
                else
                    lua.L_ArgError(1, "Not a GameObject or Component");
            }
            return 1;
        }

        /// <summary>
        /// Returns the collider of GameObject. collider GetCollider(go)
        /// </summary>
        /// <returns>transform</returns>
        private static int GetCollider(ILuaState lua) {
            object o = lua.ToUserData(1);

            Collider coll = null;

            GameObject go = o as GameObject;
            if(go)
                coll = go.collider;
            else {
                Component comp = o as Component;
                if(comp)
                    coll = comp.collider;
                else
                    lua.L_ArgError(1, "Not a GameObject or Component");
            }

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
            object o = lua.ToUserData(1);

            Rigidbody body = null;

            GameObject go = o as GameObject;
            if(go)
                body = go.rigidbody;
            else {
                Component comp = o as Component;
                if(comp)
                    body = comp.rigidbody;
                else
                    lua.L_ArgError(1, "Not a GameObject or Component");
            }

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
            object o = lua.ToUserData(1);
            GameObject go = o as GameObject;
            Component comp = null;
            if(!go) {
                comp = o as Component;
                if(!comp) {
                    lua.L_ArgError(1, "Not a GameObject or Component");
                    return 0;
                }
            }

            string method = lua.L_CheckString(2);

            if(lua.GetTop() >= 3) {
                switch(lua.Type(3)) {
                    case LuaType.LUA_TBOOLEAN:
                        if(go) go.SendMessage(method, lua.ToBoolean(3), SendMessageOptions.DontRequireReceiver);
                        else comp.SendMessage(method, lua.ToBoolean(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TNUMBER:
                        if(go) go.SendMessage(method, ((float)lua.ToNumber(3)), SendMessageOptions.DontRequireReceiver);
                        else comp.SendMessage(method, ((float)lua.ToNumber(3)), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TSTRING:
                        if(go) go.SendMessage(method, lua.ToString(3), SendMessageOptions.DontRequireReceiver);
                        else comp.SendMessage(method, lua.ToString(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TUINT64:
                        if(go) go.SendMessage(method, lua.ToInteger(3), SendMessageOptions.DontRequireReceiver);
                        else comp.SendMessage(method, lua.ToInteger(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TLIGHTUSERDATA:
                        if(go) go.SendMessage(method, lua.ToUserData(3), SendMessageOptions.DontRequireReceiver);
                        else comp.SendMessage(method, lua.ToUserData(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    default:
                        lua.L_ArgError(3, "Incompatible Type");
                        break;
                }
            }
            else {
                if(go) go.SendMessage(method, null, SendMessageOptions.DontRequireReceiver);
                else comp.SendMessage(method, null, SendMessageOptions.DontRequireReceiver);
            }
            return 0;
        }

        /// <summary>
        /// Send message. SendMessageUpwards(go, method, var)
        /// </summary>
        private static int SendMessageUpwards(ILuaState lua) {
            object o = lua.ToUserData(1);
            GameObject go = o as GameObject;
            Component comp = null;
            if(!go) {
                comp = o as Component;
                if(!comp) {
                    lua.L_ArgError(1, "Not a GameObject or Component");
                    return 0;
                }
            }

            string method = lua.L_CheckString(2);

            if(lua.GetTop() >= 3) {
                switch(lua.Type(3)) {
                    case LuaType.LUA_TBOOLEAN:
                        if(go) go.SendMessageUpwards(method, lua.ToBoolean(3), SendMessageOptions.DontRequireReceiver);
                        else comp.SendMessageUpwards(method, lua.ToBoolean(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TNUMBER:
                        if(go) go.SendMessageUpwards(method, ((float)lua.ToNumber(3)), SendMessageOptions.DontRequireReceiver);
                        else comp.SendMessageUpwards(method, ((float)lua.ToNumber(3)), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TSTRING:
                        if(go) go.SendMessageUpwards(method, lua.ToString(3), SendMessageOptions.DontRequireReceiver);
                        else comp.SendMessageUpwards(method, lua.ToString(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TUINT64:
                        if(go) go.SendMessageUpwards(method, lua.ToInteger(3), SendMessageOptions.DontRequireReceiver);
                        else comp.SendMessageUpwards(method, lua.ToInteger(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TLIGHTUSERDATA:
                        if(go) go.SendMessageUpwards(method, lua.ToUserData(3), SendMessageOptions.DontRequireReceiver);
                        else comp.SendMessageUpwards(method, lua.ToUserData(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    default:
                        lua.L_ArgError(3, "Incompatible Type");
                        break;
                }
            }
            else {
                if(go) go.SendMessageUpwards(method, null, SendMessageOptions.DontRequireReceiver);
                else comp.SendMessageUpwards(method, null, SendMessageOptions.DontRequireReceiver);
            }
            return 0;
        }

        /// <summary>
        /// Send message. BroadcastMessage(go, method, var)
        /// </summary>
        private static int BroadcastMessage(ILuaState lua) {
            object o = lua.ToUserData(1);
            GameObject go = o as GameObject;
            Component comp = null;
            if(!go) {
                comp = o as Component;
                if(!comp) {
                    lua.L_ArgError(1, "Not a GameObject or Component");
                    return 0;
                }
            }

            string method = lua.L_CheckString(2);

            if(lua.GetTop() >= 3) {
                switch(lua.Type(3)) {
                    case LuaType.LUA_TBOOLEAN:
                        if(go) go.BroadcastMessage(method, lua.ToBoolean(3), SendMessageOptions.DontRequireReceiver);
                        else comp.BroadcastMessage(method, lua.ToBoolean(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TNUMBER:
                        if(go) go.BroadcastMessage(method, ((float)lua.ToNumber(3)), SendMessageOptions.DontRequireReceiver);
                        else comp.BroadcastMessage(method, ((float)lua.ToNumber(3)), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TSTRING:
                        if(go) go.BroadcastMessage(method, lua.ToString(3), SendMessageOptions.DontRequireReceiver);
                        else comp.BroadcastMessage(method, lua.ToString(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TUINT64:
                        if(go) go.BroadcastMessage(method, lua.ToInteger(3), SendMessageOptions.DontRequireReceiver);
                        else comp.BroadcastMessage(method, lua.ToInteger(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    case LuaType.LUA_TLIGHTUSERDATA:
                        if(go) go.BroadcastMessage(method, lua.ToUserData(3), SendMessageOptions.DontRequireReceiver);
                        else comp.BroadcastMessage(method, lua.ToUserData(3), SendMessageOptions.DontRequireReceiver);
                        break;
                    default:
                        lua.L_ArgError(3, "Incompatible Type");
                        break;
                }
            }
            else {
                if(go) go.BroadcastMessage(method, null, SendMessageOptions.DontRequireReceiver);
                else comp.BroadcastMessage(method, null, SendMessageOptions.DontRequireReceiver);
            }
            return 0;
        }
    #endregion
    }
#endregion

#region Transform
    public static class UnityTransform {
        public const string LIB_NAME = "Unity.Transform";

        public static int OpenLib(ILuaState lua) {
            NameFuncPair[] funcs = new NameFuncPair[] {
                new NameFuncPair("GetParent", GetParent),
                new NameFuncPair("Find", Find),
                new NameFuncPair("GetChildCount", GetChildCount),
                new NameFuncPair("GetChild", GetChild),

                new NameFuncPair("GetPosition", GetPosition),
                new NameFuncPair("SetPosition", SetPosition),
                new NameFuncPair("GetLocalPosition", GetLocalPosition),
                new NameFuncPair("SetLocalPosition", SetLocalPosition),
                new NameFuncPair("GetRotation", GetRotation),
                new NameFuncPair("SetRotation", SetRotation),
                new NameFuncPair("GetLocalRotation", GetLocalRotation),
                new NameFuncPair("SetLocalRotation", SetLocalRotation),
                new NameFuncPair("GetLocalScale", GetLocalScale),
                new NameFuncPair("SetLocalScale", SetLocalScale),
                new NameFuncPair("GetEulerAngles", GetEulerAngles),
                new NameFuncPair("SetEulerAngles", SetEulerAngles),
                new NameFuncPair("GetLocalEulerAngles", GetLocalEulerAngles),
                new NameFuncPair("SetLocalEulerAngles", SetLocalEulerAngles),
                new NameFuncPair("GetUp", GetUp),
                new NameFuncPair("SetUp", SetUp),
                new NameFuncPair("GetRight", GetRight),
                new NameFuncPair("SetRight", SetRight),
                new NameFuncPair("GetForward", GetForward),
                new NameFuncPair("SetForward", SetForward),
            };

            lua.L_NewLib(funcs);
            return 1;
        }

        private static int GetParent(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Transform parent = t.parent;
            if(parent)
                lua.PushLightUserData(parent);
            else
                lua.PushNil();
            return 1;
        }

        private static int Find(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            string path = lua.L_CheckString(2);
            Transform c = t.Find(path);
            if(c)
                lua.PushLightUserData(c);
            else
                lua.PushNil();
            return 1;
        }

        private static int GetChildCount(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            lua.PushInteger(t.childCount);
            return 1;
        }

        private static int GetChild(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            int ind = lua.L_CheckInteger(2);
            Transform c = t.GetChild(ind);
            if(c)
                lua.PushLightUserData(c);
            else
                lua.PushNil();
            return 1;
        }

        private static int GetPosition(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 pos = t.position;
            lua.PushNumber(pos.x);
            lua.PushNumber(pos.y);
            lua.PushNumber(pos.z);
            return 3;
        }

        private static int SetPosition(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            t.position = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            return 0;
        }

        private static int GetLocalPosition(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 pos = t.localPosition;
            lua.PushNumber(pos.x);
            lua.PushNumber(pos.y);
            lua.PushNumber(pos.z);
            return 3;
        }

        private static int SetLocalPosition(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            t.localPosition = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            return 0;
        }

        private static int GetRotation(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Quaternion q = t.rotation;
            lua.PushNumber(q.x);
            lua.PushNumber(q.y);
            lua.PushNumber(q.z);
            lua.PushNumber(q.w);
            return 4;
        }

        private static int SetRotation(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            t.rotation = new Quaternion((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4), (float)lua.L_CheckNumber(5));
            return 0;
        }

        private static int GetLocalRotation(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Quaternion q = t.localRotation;
            lua.PushNumber(q.x);
            lua.PushNumber(q.y);
            lua.PushNumber(q.z);
            lua.PushNumber(q.w);
            return 4;
        }

        private static int SetLocalRotation(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            t.localRotation = new Quaternion((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4), (float)lua.L_CheckNumber(5));
            return 0;
        }

        private static int GetLocalScale(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 s = t.localScale;
            lua.PushNumber(s.x);
            lua.PushNumber(s.y);
            lua.PushNumber(s.z);
            return 3;
        }

        private static int SetLocalScale(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            t.localScale = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            return 0;
        }

        private static int GetEulerAngles(ILuaState lua) { //in degrees
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 e = t.eulerAngles;
            lua.PushNumber(e.x);
            lua.PushNumber(e.y);
            lua.PushNumber(e.z);
            return 3;
        }

        private static int SetEulerAngles(ILuaState lua) { //in degrees
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            t.eulerAngles = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            return 0;
        }

        private static int GetLocalEulerAngles(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 e = t.localEulerAngles;
            lua.PushNumber(e.x);
            lua.PushNumber(e.y);
            lua.PushNumber(e.z);
            return 3;
        }

        private static int SetLocalEulerAngles(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            t.localEulerAngles = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            return 0;
        }

        private static int GetUp(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 up = t.up;
            lua.PushNumber(up.x);
            lua.PushNumber(up.y);
            lua.PushNumber(up.z);
            return 3;
        }

        private static int SetUp(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            t.up = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            return 0;
        }

        private static int GetRight(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 right = t.right;
            lua.PushNumber(right.x);
            lua.PushNumber(right.y);
            lua.PushNumber(right.z);
            return 3;
        }

        private static int SetRight(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            t.right = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            return 0;
        }

        private static int GetForward(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 forward = t.forward;
            lua.PushNumber(forward.x);
            lua.PushNumber(forward.y);
            lua.PushNumber(forward.z);
            return 3;
        }

        private static int SetForward(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            t.forward = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            return 0;
        }
    }
#endregion
}