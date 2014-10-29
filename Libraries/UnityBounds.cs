using UnityEngine;

using UniLua;

namespace M8.Lua.Library {
    /// <summary>
    /// Used as a wrapper for Bounds
    /// </summary>
    public class UnityBounds : IContainer {
        public const string LIB_NAME = "Unity.Bounds";

        private static NameFuncPair[] m_funcs = null;
        private static NameFuncPair[] l_funcs = null;

        public static int OpenLib(ILuaState lua) {
            if(m_funcs == null)
                m_funcs = new NameFuncPair[] {
                    new NameFuncPair("__tostring", ToString),

                    new NameFuncPair("getCenter", GetCenter),
                    new NameFuncPair("setCenter", SetCenter),
                    new NameFuncPair("getExtents", GetExtents),
                    new NameFuncPair("setExtents", SetExtents),
                    new NameFuncPair("getMax", GetMax),
                    new NameFuncPair("setMax", SetMax),
                    new NameFuncPair("getMin", GetMin),
                    new NameFuncPair("setMin", SetMin),
                    new NameFuncPair("getSize", GetSize),
                    new NameFuncPair("setSize", SetSize),

                    new NameFuncPair("contains", Contains),
                    new NameFuncPair("encapsulate", Encapsulate),
                    new NameFuncPair("expand", Expand),
                    new NameFuncPair("intersectRay", IntersectRay),
                    new NameFuncPair("intersects", Intersects),
                    new NameFuncPair("sqrDistance", SqrDistance),
                };
            if(l_funcs == null)
                l_funcs = new NameFuncPair[] {
                    new NameFuncPair("New", New),
                };

            Utils.NewMeta(lua, typeof(UnityBounds), m_funcs);
            lua.L_NewLib(l_funcs);
            return 1;
        }

        private static int New(ILuaState lua) {
            UnityBounds b = new UnityBounds(new Bounds(Vector3.zero, Vector3.zero));
            lua.NewUserData(b);
            Utils.SetMetaTableByType(lua, typeof(UnityBounds));
            return 1;
        }

        private static int ToString(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null)
                lua.PushString(b.mBounds.ToString());
            else
                lua.L_ArgError(1, "Not UnityBounds");
            return 1;
        }

        private static int GetCenter(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null) {
                Vector3 c = b.mBounds.center;
                lua.PushNumber(c.x);
                lua.PushNumber(c.y);
                lua.PushNumber(c.z);
            }
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 3;
        }

        private static int SetCenter(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null)
                b.mBounds.center = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 0;
        }

        private static int GetExtents(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null) {
                Vector3 e = b.mBounds.extents;
                lua.PushNumber(e.x);
                lua.PushNumber(e.y);
                lua.PushNumber(e.z);
            }
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 3;
        }

        private static int SetExtents(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null)
                b.mBounds.extents = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 0;
        }

        private static int GetMax(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null) {
                Vector3 max = b.mBounds.max;
                lua.PushNumber(max.x);
                lua.PushNumber(max.y);
                lua.PushNumber(max.z);
            }
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 3;
        }

        private static int SetMax(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null)
                b.mBounds.max = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 0;
        }

        private static int GetMin(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null) {
                Vector3 min = b.mBounds.min;
                lua.PushNumber(min.x);
                lua.PushNumber(min.y);
                lua.PushNumber(min.z);
            }
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 3;
        }

        private static int SetMin(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null)
                b.mBounds.min = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 0;
        }

        private static int GetSize(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null) {
                Vector3 size = b.mBounds.size;
                lua.PushNumber(size.x);
                lua.PushNumber(size.y);
                lua.PushNumber(size.z);
            }
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 3;
        }

        private static int SetSize(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null)
                b.mBounds.size = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 0;
        }

        private static int Contains(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null)
                lua.PushBoolean(b.mBounds.Contains(new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4))));
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 1;
        }

        /// <summary>
        /// Either pass a UnityBounds or x, y, z
        /// </summary>
        private static int Encapsulate(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null) {
                if(lua.Type(2) == LuaType.LUA_TUSERDATA) {
                    UnityBounds otherB = lua.ToUserData(2) as UnityBounds;
                    if(otherB != null)
                        b.mBounds.Encapsulate(otherB.mBounds);
                    else
                        lua.L_ArgError(2, "Not UnityBounds");
                }
                else
                    b.mBounds.Encapsulate(new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4)));
            }
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 0;
        }

        private static int Expand(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null) {
                int numArgs = lua.GetTop() - 1;
                if(numArgs == 1)
                    b.mBounds.Expand((float)lua.L_CheckNumber(2));
                else
                    b.mBounds.Expand(new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4)));
            }
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 0;
        }

        /// <summary>
        /// bool, dist = bounds:IntersectRay(pt = {x, y, z}, dir = {x, y, z})
        /// </summary>
        private static int IntersectRay(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null) {
                Vector3 pt = Utils.TableToVector3(lua, 2);
                Vector3 dir = Utils.TableToVector3(lua, 3);
                float dist;
                lua.PushBoolean(b.mBounds.IntersectRay(new Ray(pt, dir), out dist));
                lua.PushNumber(dist);
            }
            else
                lua.L_ArgError(1, "Not UnityBounds");
            return 2;
        }

        private static int Intersects(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null) {
                UnityBounds otherB = lua.ToUserData(2) as UnityBounds;
                if(otherB != null)
                    lua.PushBoolean(b.mBounds.Intersects(otherB.mBounds));
                else
                    lua.L_ArgError(2, "Not UnityBounds");
            }
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 1;
        }

        private static int SqrDistance(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null)
                lua.PushNumber(b.mBounds.SqrDistance(new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4))));
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 1;
        }

        public object data { get { return mBounds; } }

        public UnityBounds(Bounds b) { mBounds = b; }

        private Bounds mBounds;
    }
}