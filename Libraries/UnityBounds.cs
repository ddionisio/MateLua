using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    /// <summary>
    /// Used as a wrapper for Bounds
    /// </summary>
    public class UnityBounds {
        public const string META_NAME = "bounds_mt";
        public const string LIB_NAME = "Unity.Bounds";

        public static int OpenLib(ILuaState lua) {
            NameFuncPair[] l_funcs = new NameFuncPair[] {
                new NameFuncPair("New", New),
            };

            NameFuncPair[] m_funcs = new NameFuncPair[] {
                new NameFuncPair("GetCenter", GetCenter),
                new NameFuncPair("SetCenter", SetCenter),
            };

            lua.NewMetaTable(META_NAME);

            lua.PushString("__index");
            lua.PushValue(-2); //meta
            lua.SetTable(-3); //meta.__index = meta
            
            lua.L_SetFuncs(m_funcs, 0);

            lua.L_NewLib(l_funcs);
            return 1;
        }

        public static int New(ILuaState lua) {
            UnityBounds b = new UnityBounds(new Bounds(Vector3.one, Vector3.one));
            lua.PushLightUserData(b);

            lua.GetMetaTable(META_NAME);
            lua.SetMetaTable(-2);
            return 1;
        }

        private static int GetCenter(ILuaState lua) {
            UnityBounds b = lua.ToUserData(1) as UnityBounds;
            if(b != null) {
                Vector3 c = b.bounds.center;
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
            if(b != null) {
                b.mBounds.center = new Vector3((float)lua.L_CheckNumber(2), (float)lua.L_CheckNumber(3), (float)lua.L_CheckNumber(4));
            }
            else
                lua.L_ArgError(1, "Not UnityBounds");

            return 0;
        }

        public Bounds bounds { get { return mBounds; } }

        public UnityBounds(Bounds b) { mBounds = b; }

        private Bounds mBounds;
    }
}