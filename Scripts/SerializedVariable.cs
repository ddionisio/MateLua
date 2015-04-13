using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua {
    [System.Serializable]
    public struct SerializedVariable {
        public enum Type {
            Boolean,
            Integer,
            Float,
            Vector2,
            Vector3,
            Vector4,
            Rotation,
            String,
            GameObject,
        }

        public string name;
        public Type type;

        public int iVal;
        public Vector4 vVal;
        public string sVal;
        public GameObject goVal;

        public void Reset() {
            iVal = 0;
            vVal = Vector4.zero;
            sVal = "";
            goVal = null;
        }

        public void AddToTable(Table t) {
            switch(type) {
                case Type.Boolean:
                    t[name] = iVal > 0;
                    break;
                case Type.Integer:
                    t[name] = iVal;
                    break;
                case Type.Float:
                    t[name] = vVal.x;
                    break;
                case Type.Vector2:
                    t[name] = new Vector2(vVal.x, vVal.y);
                    break;
                case Type.Vector3:
                    t[name] = new Vector3(vVal.x, vVal.y, vVal.z);
                    break;
                case Type.Vector4:
                    t[name] = vVal;
                    break;
                case Type.Rotation:
                    t[name] = Quaternion.Euler(vVal.x, vVal.y, vVal.z);
                    break;
                case Type.String:
                    t[name] = sVal;
                    break;
                case Type.GameObject:
                    t[name] = goVal;
                    break;
            }
        }
    }
}