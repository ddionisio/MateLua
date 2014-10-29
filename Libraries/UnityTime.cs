using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class UnityTime {
        public const string LIB_NAME = "Unity.Time";

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
                };

            Utils.NewMetaGetterSetter(lua, typeof(Time), m_funcs);
            lua.L_NewLib(l_funcs);

            Utils.SetMetaTableByType(lua, typeof(Time));

            return 1;
        }

        private static int Get(ILuaState lua) {
            string field = lua.L_CheckString(2);
            switch(field) {
                case "captureFramerate":
                    lua.PushInteger(Time.captureFramerate);
                    break;
                case "deltaTime":
                    lua.PushNumber(Time.deltaTime);
                    break;
                case "fixedDeltaTime":
                    lua.PushNumber(Time.fixedDeltaTime);
                    break;
                case "fixedTime":
                    lua.PushNumber(Time.fixedTime);
                    break;
                case "frameCount":
                    lua.PushNumber(Time.frameCount);
                    break;
                case "maximumDeltaTime":
                    lua.PushNumber(Time.maximumDeltaTime);
                    break;
                case "realtimeSinceStartup":
                    lua.PushNumber(Time.realtimeSinceStartup);
                    break;
                case "renderedFrameCount":
                    lua.PushNumber(Time.renderedFrameCount);
                    break;
                case "smoothDeltaTime":
                    lua.PushNumber(Time.smoothDeltaTime);
                    break;
                case "time":
                    lua.PushNumber(Time.time);
                    break;
                case "timeScale":
                    lua.PushNumber(Time.timeScale);
                    break;
                case "timeSinceLevelLoad":
                    lua.PushNumber(Time.timeSinceLevelLoad);
                    break;
                case "unscaledDeltaTime":
                    lua.PushNumber(Time.unscaledDeltaTime);
                    break;
                case "unscaledTime":
                    lua.PushNumber(Time.unscaledTime);
                    break;
                default:
                    if(!lua.L_GetMetaField(1, field))
                        lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 1;
        }

        private static int Set(ILuaState lua) {
            string field = lua.L_CheckString(2);
            switch(field) {
                case "captureFramerate":
                    Time.captureFramerate = lua.L_CheckInteger(3);
                    break;
                case "fixedDeltaTime":
                    Time.fixedDeltaTime = (float)lua.L_CheckNumber(3);
                    break;
                case "maximumDeltaTime":
                    Time.maximumDeltaTime = (float)lua.L_CheckNumber(3);
                    break;
                case "timeScale":
                    Time.timeScale = (float)lua.L_CheckNumber(3);
                    break;
                default:
                    lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 0;
        }
    }
}