using UnityEngine;

using UniLua;

namespace M8.Lua.Library {
    public static class UnityCollision {
        public const string META_NAME = "Unity.Collision.Meta";
        public const string LIB_NAME = "Unity.Collision";

        private static NameFuncPair[] m_funcs = null;
        private static NameFuncPair[] l_funcs = null;

        public static int OpenLib(ILuaState lua) {
            if(m_funcs == null)
                m_funcs = new NameFuncPair[] {
                    new NameFuncPair(Utils.GETTER, Get),
                    new NameFuncPair(Utils.SETTER, Set),

                    new NameFuncPair("getRelativeVelocity", GetRelativeVelocity),
                };
            if(l_funcs == null)
                l_funcs = new NameFuncPair[] {
                };

            Utils.NewMetaGetterSetter(lua, typeof(Collision), m_funcs);
            lua.L_NewLib(l_funcs);
                        
            return 1;
        }

        private static int Get(ILuaState lua) {
            Collision coll = lua.ToUserData(1) as Collision;
            if(coll == null) { lua.L_ArgError(1, "Not a Collision"); }

            string field = lua.L_CheckString(2);
            switch(field) {
                case "collider":
                    if(coll.collider) { lua.NewUserData(coll.collider); Utils.SetMetaTableByType(lua, typeof(Collider)); }
                    else lua.PushNil();
                    break;
                case "contacts":
                    ContactPoint[] cps = coll.contacts;
                    lua.CreateTable(cps.Length, 0);
                    for(int i = 0; i < cps.Length; i++) {
                        lua.NewUserData(cps[i]);
                        lua.SetMetaTable(UnityContactPoint.META_NAME);
                        lua.RawSetI(-2, i+1);
                    }
                    break;
                case "gameObject":
                    UnityGameObject.Push(lua, coll.gameObject);
                    break;
                case "rigidbody":
                    UnityRigidbody.Push(lua, coll.rigidbody);
                    break;
                case "transform":
                    UnityTransform.Push(lua, coll.transform);
                    break;
                default:
                    if(!lua.L_GetMetaField(1, field))
                        lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 1;
        }

        private static int Set(ILuaState lua) {
            lua.L_Error("Access Denied!");
            return 0;
        }

        private static int GetRelativeVelocity(ILuaState lua) {
            Collision coll = lua.ToUserData(1) as Collision;
            if(coll == null) { lua.L_ArgError(1, "Not a Collision"); }

            Vector3 relV = coll.relativeVelocity;
            lua.PushNumber(relV.x);
            lua.PushNumber(relV.y);
            lua.PushNumber(relV.z);
            return 3;
        }
    }

    public static class UnityContactPoint {
        public const string META_NAME = "Unity.ContactPoint.Meta";

        private static NameFuncPair[] m_funcs = null;

        public static void DefineMeta(ILuaState lua) {
            if(m_funcs == null)
                m_funcs = new NameFuncPair[] {
                    new NameFuncPair(Utils.GETTER, Get),
                    new NameFuncPair(Utils.SETTER, Set),

                    new NameFuncPair("getNormal", GetNormal),
                    new NameFuncPair("getPoint", GetPoint),
                };

            Utils.NewMetaGetterSetter(lua, typeof(ContactPoint), m_funcs);
        }

        private static int Get(ILuaState lua) {
            try {
                ContactPoint cp = (ContactPoint)lua.ToUserData(1);
                
                string field = lua.L_CheckString(2);
                switch(field) {
                    case "otherCollider":
                        UnityCollider.Push(lua, cp.otherCollider);
                        break;
                    case "thisCollider":
                        UnityCollider.Push(lua, cp.thisCollider);
                        break;
                    default:
                        if(!lua.L_GetMetaField(1, field))
                            lua.L_Error("Unknown field: {0}", field);
                        break;
                }
            }
            catch(System.InvalidCastException e) {
                lua.L_ArgError(1, e.ToString());
            }
            return 1;
        }

        private static int Set(ILuaState lua) {
            lua.L_Error("Access Denied!");
            return 0;
        }

        private static int GetNormal(ILuaState lua) {
            try {
                ContactPoint cp = (ContactPoint)lua.ToUserData(1);

                Vector3 n = cp.normal;
                lua.PushNumber(n.x);
                lua.PushNumber(n.y);
                lua.PushNumber(n.z);
            }
            catch(System.InvalidCastException e) {
                lua.L_ArgError(1, e.ToString());
            }
            return 3;
        }

        private static int GetPoint(ILuaState lua) {
            try {
                ContactPoint cp = (ContactPoint)lua.ToUserData(1);

                Vector3 p = cp.point;
                lua.PushNumber(p.x);
                lua.PushNumber(p.y);
                lua.PushNumber(p.z);
            }
            catch(System.InvalidCastException e) {
                lua.L_ArgError(1, e.ToString());
            }
            return 3;
        }
    }
}