using UnityEngine;

using UniLua;

namespace M8.Lua.Library {
    public static class UnityRaycastHit {
        public const string META_NAME = "Unity.RaycastHit.Meta";

        private static NameFuncPair[] m_funcs = null;

        public static void DefineMeta(ILuaState lua) {
            if(m_funcs == null)
                m_funcs = new NameFuncPair[] {
                    new NameFuncPair(Utils.GETTER, Get),
                    new NameFuncPair(Utils.SETTER, Set),

                    new NameFuncPair("getBarycentricCoordinate", GetBarycentricCoordinate),
                    new NameFuncPair("getLightmapCoord", GetLightmapCoord),
                    new NameFuncPair("getNormal", GetNormal),
                    new NameFuncPair("getPoint", GetPoint),
                    new NameFuncPair("getTextureCoord", GetTextureCoord),
                    new NameFuncPair("getTextureCoord2", GetTextureCoord2),
                };

            Utils.NewMetaGetterSetter(lua, META_NAME, m_funcs);
        }

        private static int Get(ILuaState lua) {
            try {
                RaycastHit hit = (RaycastHit)lua.ToUserData(1);

                string field = lua.L_CheckString(2);
                switch(field) {
                    case "collider":
                        if(hit.collider) {
                            lua.NewUserData(hit.collider);
                            lua.SetMetaTable(UnityCollider.META_NAME);
                        }
                        else
                            lua.PushNil();
                        break;
                    case "distance":
                        lua.PushNumber(hit.distance);
                        break;
                    case "rigidbody":
                        if(hit.rigidbody) {
                            lua.NewUserData(hit.rigidbody);
                            lua.SetMetaTable(UnityRigidbody.META_NAME);
                        }
                        else
                            lua.PushNil();
                        break;
                    case "transform":
                        if(hit.transform) {
                            lua.NewUserData(hit.transform);
                            lua.SetMetaTable(UnityTransform.META_NAME);
                        }
                        else
                            lua.PushNil();
                        break;
                    case "triangleIndex":
                        lua.PushInteger(hit.triangleIndex);
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

        private static int GetBarycentricCoordinate(ILuaState lua) {
            try {
                RaycastHit hit = (RaycastHit)lua.ToUserData(1);

                Vector3 b = hit.barycentricCoordinate;
                lua.PushNumber(b.x);
                lua.PushNumber(b.y);
                lua.PushNumber(b.z);
            }
            catch(System.InvalidCastException e) {
                lua.L_ArgError(1, e.ToString());
            }
            return 3;
        }

        private static int GetLightmapCoord(ILuaState lua) {
            try {
                RaycastHit hit = (RaycastHit)lua.ToUserData(1);

                Vector2 uv = hit.lightmapCoord;
                lua.PushNumber(uv.x);
                lua.PushNumber(uv.y);
            }
            catch(System.InvalidCastException e) {
                lua.L_ArgError(1, e.ToString());
            }
            return 2;
        }

        private static int GetNormal(ILuaState lua) {
            try {
                RaycastHit hit = (RaycastHit)lua.ToUserData(1);

                Vector3 n = hit.normal;
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
                RaycastHit hit = (RaycastHit)lua.ToUserData(1);

                Vector3 p = hit.point;
                lua.PushNumber(p.x);
                lua.PushNumber(p.y);
                lua.PushNumber(p.z);
            }
            catch(System.InvalidCastException e) {
                lua.L_ArgError(1, e.ToString());
            }
            return 3;
        }

        private static int GetTextureCoord(ILuaState lua) {
            try {
                RaycastHit hit = (RaycastHit)lua.ToUserData(1);

                Vector2 uv = hit.textureCoord;
                lua.PushNumber(uv.x);
                lua.PushNumber(uv.y);
            }
            catch(System.InvalidCastException e) {
                lua.L_ArgError(1, e.ToString());
            }
            return 2;
        }

        private static int GetTextureCoord2(ILuaState lua) {
            try {
                RaycastHit hit = (RaycastHit)lua.ToUserData(1);

                Vector2 uv = hit.textureCoord2;
                lua.PushNumber(uv.x);
                lua.PushNumber(uv.y);
            }
            catch(System.InvalidCastException e) {
                lua.L_ArgError(1, e.ToString());
            }
            return 2;
        }
    }
}