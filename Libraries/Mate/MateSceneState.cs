using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class MateSceneStateCommon {
        public static int ClearAllSavedData(ILuaState lua) {
            bool resetValues = lua.GetTop() >= 2 ? lua.ToBoolean(2) : true;
            SceneState.instance.ClearAllSavedData(resetValues);
            return 0;
        }

        public static int DeleteValuesByNameContain(ILuaState lua) {
            string nameContains = lua.L_CheckString(2);
            SceneState.instance.DeleteValuesByNameContain(nameContains);
            return 0;
        }

        public static int ResetValues(ILuaState lua) {
            SceneState.instance.ResetValues();
            return 0;
        }
    }

    public static class MateSceneState {
        public const string LIB_NAME = "Mate.SceneState";

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
                    new NameFuncPair("DeleteValuesByNameContain", MateSceneStateCommon.DeleteValuesByNameContain),
                    new NameFuncPair("ClearAllSavedData", MateSceneStateCommon.ClearAllSavedData),
                    new NameFuncPair("ResetValues", MateSceneStateCommon.ResetValues),
                    new NameFuncPair("SetValue", SetValue),
                    new NameFuncPair("SetPersist", SetPersist),
                    new NameFuncPair("CheckFlag", CheckFlag),
                    new NameFuncPair("CheckFlagMask", CheckFlagMask),
                    new NameFuncPair("SetFlag", SetFlag),
                };

            Utils.NewMetaGetterSetter(lua, typeof(MateSceneState), m_funcs);
            lua.L_NewLib(l_funcs);

            Utils.SetMetaTableByType(lua, typeof(MateSceneState));

            return 1;
        }

        private static int Get(ILuaState lua) {
            string field = lua.L_CheckString(2);
            SceneState ss = SceneState.instance;
            SceneState.StateValue val = ss.GetValueRaw(field);
            if(val.type != SceneState.Type.Invalid) {
                switch(val.type) {
                    case SceneState.Type.Float:
                        lua.PushNumber(val.fval);
                        break;
                    case SceneState.Type.Integer:
                        lua.PushInteger(val.ival);
                        break;
                    case SceneState.Type.String:
                        lua.PushString(val.sval);
                        break;
                }
            }
            else if(!lua.L_GetMetaField(1, field))
                lua.PushNil();

            return 1;
        }

        private static int Set(ILuaState lua) {
            string field = lua.L_CheckString(2);

            SceneState ss = SceneState.instance;

            if(lua.Type(3) == LuaType.LUA_TSTRING)
                ss.SetValueString(field, lua.ToString(3), false);
            else if(lua.Type(3) == LuaType.LUA_TNUMBER)
                ss.SetValueFloat(field, (float)lua.ToNumber(3), false);
            else if(lua.IsNil(3))
                ss.DeleteValue(field, false);
            else
                lua.L_ArgError(3, "Not a number or string.");

            return 0;
        }

        private static int SetValue(ILuaState lua) {
            string field = lua.L_CheckString(1);
            SceneState.instance.SetValue(field, lua.L_CheckInteger(2), lua.GetTop() >= 3 ? lua.ToBoolean(3) : false);
            return 0;
        }

        private static int SetPersist(ILuaState lua) {
            string field = lua.L_CheckString(2);
            bool persist = lua.ToBoolean(3);
            SceneState.instance.SetPersist(field, persist);
            return 0;
        }
        
        private static int CheckFlag(ILuaState lua) {
            string field = lua.L_CheckString(2);
            int bit = lua.L_CheckInteger(3);
            bool isSet = SceneState.instance.CheckFlag(field, bit);
            lua.PushBoolean(isSet);
            return 1;
        }

        private static int CheckFlagMask(ILuaState lua) {
            string field = lua.L_CheckString(2);
            int mask = lua.L_CheckInteger(3);
            bool isSet = SceneState.instance.CheckFlagMask(field, mask);
            lua.PushBoolean(isSet);
            return 1;
        }

        private static int SetFlag(ILuaState lua) {
            string field = lua.L_CheckString(2);
            int bit = lua.L_CheckInteger(3);
            bool state = lua.ToBoolean(4);
            bool persistent = lua.ToBoolean(5);
            SceneState.instance.SetFlag(field, bit, state, persistent);
            return 0;
        }
    }

    public static class MateGlobalState {
        public const string LIB_NAME = "Mate.GlobalState";

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
                    new NameFuncPair("DeleteValuesByNameContain", MateSceneStateCommon.DeleteValuesByNameContain),
                    new NameFuncPair("ClearAllSavedData", MateSceneStateCommon.ClearAllSavedData),
                    new NameFuncPair("ResetValues", MateSceneStateCommon.ResetValues),
                    new NameFuncPair("SetValue", SetValue),
                    new NameFuncPair("SetPersist", SetPersist),
                    new NameFuncPair("CheckFlag", CheckFlag),
                    new NameFuncPair("CheckFlagMask", CheckFlagMask),
                    new NameFuncPair("SetFlag", SetFlag),
                    new NameFuncPair("SnapShotSave", SnapShotSave),
                    new NameFuncPair("SnapShotRestore", SnapShotRestore),
                    new NameFuncPair("SnapShotDelete", SnapShotDelete),
                };

            Utils.NewMetaGetterSetter(lua, typeof(MateGlobalState), m_funcs);
            lua.L_NewLib(l_funcs);

            Utils.SetMetaTableByType(lua, typeof(MateGlobalState));

            return 1;
        }

        private static int Get(ILuaState lua) {
            string field = lua.L_CheckString(2);
            SceneState ss = SceneState.instance;
            SceneState.StateValue val = ss.GetGlobalValueRaw(field);
            if(val.type != SceneState.Type.Invalid) {
                switch(val.type) {
                    case SceneState.Type.Float:
                        lua.PushNumber(val.fval);
                        break;
                    case SceneState.Type.Integer:
                        lua.PushInteger(val.ival);
                        break;
                    case SceneState.Type.String:
                        lua.PushString(val.sval);
                        break;
                }
            }
            else if(!lua.L_GetMetaField(1, field))
                lua.PushNil();

            return 1;
        }

        private static int Set(ILuaState lua) {
            string field = lua.L_CheckString(2);

            SceneState ss = SceneState.instance;

            if(lua.Type(3) == LuaType.LUA_TSTRING)
                ss.SetGlobalValueString(field, lua.ToString(3), false);
            else if(lua.Type(3) == LuaType.LUA_TNUMBER)
                ss.SetGlobalValueFloat(field, (float)lua.ToNumber(3), false);
            else if(lua.IsNil(3))
                ss.DeleteGlobalValue(field, false);
            else
                lua.L_ArgError(3, "Not a number or string.");

            return 0;
        }

        private static int SetValue(ILuaState lua) {
            string field = lua.L_CheckString(1);
            SceneState.instance.SetGlobalValue(field, lua.L_CheckInteger(2), lua.GetTop() >= 3 ? lua.ToBoolean(3) : false);
            return 0;
        }

        private static int SetPersist(ILuaState lua) {
            string field = lua.L_CheckString(2);
            bool persist = lua.ToBoolean(3);
            SceneState.instance.SetGlobalPersist(field, persist);
            return 0;
        }

        private static int CheckFlag(ILuaState lua) {
            string field = lua.L_CheckString(2);
            int bit = lua.L_CheckInteger(3);
            bool isSet = SceneState.instance.CheckGlobalFlag(field, bit);
            lua.PushBoolean(isSet);
            return 1;
        }

        private static int CheckFlagMask(ILuaState lua) {
            string field = lua.L_CheckString(2);
            int mask = lua.L_CheckInteger(3);
            bool isSet = SceneState.instance.CheckGlobalFlagMask(field, mask);
            lua.PushBoolean(isSet);
            return 1;
        }

        private static int SetFlag(ILuaState lua) {
            string field = lua.L_CheckString(2);
            int bit = lua.L_CheckInteger(3);
            bool state = lua.ToBoolean(4);
            bool persistent = lua.ToBoolean(5);
            SceneState.instance.SetGlobalFlag(field, bit, state, persistent);
            return 0;
        }

        private static int SnapShotSave(ILuaState lua) {
            SceneState.instance.GlobalSnapshotSave();
            return 0;
        }

        private static int SnapShotRestore(ILuaState lua) {
            SceneState.instance.GlobalSnapshotRestore();
            return 0;
        }

        private static int SnapShotDelete(ILuaState lua) {
            SceneState.instance.GlobalSnapshotDelete();
            return 0;
        }
    }
}