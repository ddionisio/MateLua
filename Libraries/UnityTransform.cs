using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class UnityTransform {
        public const string LIB_NAME = "Unity.Transform";

        public static int OpenLib(ILuaState lua) {
            NameFuncPair[] funcs = new NameFuncPair[] {
                new NameFuncPair("GetParent", GetParent),
                new NameFuncPair("Find", Find),
                new NameFuncPair("GetChildCount", GetChildCount),
                new NameFuncPair("GetChild", GetChild),
                new NameFuncPair("IsChildOf", IsChildOf),

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

                new NameFuncPair("InverseTransformDirection", InverseTransformDirection),
                new NameFuncPair("InverseTransformPoint", InverseTransformPoint),
                new NameFuncPair("LookAt", LookAt),
                new NameFuncPair("RotateEulerLocal", RotateEulerLocal),
                new NameFuncPair("RotateEulerWorld", RotateEulerWorld),
                new NameFuncPair("RotateAxisLocal", RotateAxisLocal),
                new NameFuncPair("RotateAxisWorld", RotateAxisWorld),
                new NameFuncPair("TransformDirection", TransformDirection),
                new NameFuncPair("TransformPoint", TransformPoint),
                new NameFuncPair("TranslateLocal", TranslateLocal),
                new NameFuncPair("TranslateWorld", TranslateWorld),
                new NameFuncPair("TranslateRelativeTo", TranslateRelativeTo),
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

        private static int IsChildOf(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Transform otherT = Utils.CheckUnityObject<Transform>(lua, 2);
            lua.PushBoolean(t.IsChildOf(otherT));
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

        private static int InverseTransformDirection(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 d = t.InverseTransformDirection((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            lua.PushNumber(d.x);
            lua.PushNumber(d.y);
            lua.PushNumber(d.z);
            return 3;
        }

        private static int InverseTransformPoint(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 p = t.InverseTransformPoint((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            lua.PushNumber(p.x);
            lua.PushNumber(p.y);
            lua.PushNumber(p.z);
            return 3;
        }

        /// <summary>
        /// LookAt(transform)
        /// LookAt(transform, worldUpX, worldUpY, worldUpZ)
        /// LookAt(lookAtX, lookAtY, lookAtZ)
        /// LookAt(lookAtX, lookAtY, lookAtZ, worldUpX, worldUpY, worldUpZ)
        /// </summary>
        private static int LookAt(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);

            int nOtherArgs = lua.GetTop() - 1;

            if(lua.Type(2) == LuaType.LUA_TLIGHTUSERDATA) {
                Transform lookAtT = Utils.CheckUnityObject<Transform>(lua, 2);
                if(nOtherArgs - 1 >= 3) {
                    Vector3 worldUp = new Vector3((float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4), (float)lua.L_CheckNumber(5));
                    t.LookAt(lookAtT, worldUp);
                }
                else
                    t.LookAt(lookAtT);
            }
            else if(nOtherArgs >= 3) { //vector
                Vector3 look = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
                if(nOtherArgs - 3 >= 3) {
                    Vector3 worldUp = new Vector3((float)lua.L_CheckNumber(5), (float)lua.L_CheckNumber(6), (float)lua.L_CheckNumber(7));
                    t.LookAt(look, worldUp);
                }
                else
                    t.LookAt(look);
            }
            return 0;
        }

        private static int RotateEulerLocal(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 euler = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            t.Rotate(euler);
            return 0;
        }

        private static int RotateEulerWorld(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 euler = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            t.Rotate(euler, Space.World);
            return 0;
        }

        private static int RotateAxisLocal(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 axis = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            float angle = (float)lua.L_CheckNumber(5);
            t.Rotate(axis, angle);
            return 0;
        }

        private static int RotateAxisWorld(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 axis = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            float angle = (float)lua.L_CheckNumber(5);
            t.Rotate(axis, angle, Space.World);
            return 0;
        }

        private static int TransformDirection(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 d = t.TransformDirection((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            lua.PushNumber(d.x);
            lua.PushNumber(d.y);
            lua.PushNumber(d.z);
            return 3;
        }

        private static int TransformPoint(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Vector3 p = t.TransformPoint((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            lua.PushNumber(p.x);
            lua.PushNumber(p.y);
            lua.PushNumber(p.z);
            return 3;
        }

        private static int TranslateLocal(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            t.Translate((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            return 0;
        }

        private static int TranslateWorld(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            t.Translate((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4), Space.World);
            return 0;
        }

        /// <summary>
        /// TranslateRelativeTo(transform, relative transform, x, y, z)
        /// </summary>
        private static int TranslateRelativeTo(ILuaState lua) {
            Transform t = Utils.CheckUnityObject<Transform>(lua, 1);
            Transform relativeTo = Utils.CheckUnityObject<Transform>(lua, 2);
            t.Translate((float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4), (float)lua.L_CheckNumber(5), relativeTo);
            return 0;
        }
    }
}
