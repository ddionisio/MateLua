using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MoonSharp.Interpreter;

namespace M8.Lua.Modules {
    /// <summary>
    /// Note: for events, use .add and .remove (valueChangeCallback)
    /// </summary>
    public class MateSceneStateModule {
        public delegate void OnStateChange(string key, DynValue val);

        private SceneState.Table mTable;

        public event OnStateChange valueChangeCallback;

        public DynValue this[DynValue index] {
            get {
                string key = index.CastToString();
                if(!string.IsNullOrEmpty(key)) {
                    SceneState.StateValue val = mTable.GetValueRaw(key);
                    switch(val.type) {
                        case SceneState.Type.Float:
                            return DynValue.NewNumber(val.fval);
                        case SceneState.Type.Integer:
                            return DynValue.NewNumber(val.ival);
                        case SceneState.Type.String:
                            return DynValue.NewString(val.sval);
                        case SceneState.Type.Invalid:
                            break;
                    }
                }

                return DynValue.NewNil();
            }

            set {
                string key = index.CastToString();
                if(!string.IsNullOrEmpty(key)) {
                    switch(value.Type) {
                        case DataType.Number:
                            mTable.SetValueFloat(key, System.Convert.ToSingle(value.Number), false);
                            break;
                        case DataType.String:
                            mTable.SetValueString(key, value.String, false);
                            break;
                    }
                }
            }
        }

        public DynValue this[DynValue index, bool persistence] {
            set {
                string key = index.CastToString();
                if(!string.IsNullOrEmpty(key)) {
                    switch(value.Type) {
                        case DataType.Number:
                            mTable.SetValueFloat(key, System.Convert.ToSingle(value.Number), persistence);
                            break;
                        case DataType.String:
                            mTable.SetValueString(key, value.String, persistence);
                            break;
                    }
                }
            }
        }

        public void SetInt(string name, int val, bool persistent = false) {
            mTable.SetValue(name, val, persistent);
        }

        public bool CheckFlag(string name, int bit) {
            return mTable.CheckFlag(name, bit);
        }

        public bool CheckFlagMask(string name, int mask) {
            return mTable.CheckFlagMask(name, mask);
        }

        public void SetFlag(string name, int bit, bool state, bool persistent = false) {
            mTable.SetFlag(name, bit, state, persistent);
        }

        public void Reset() {
            mTable.Reset();
        }

        public void DeleteValue(string name, bool persistent) {
            mTable.DeleteValue(name, persistent);
        }

        public void Clear(bool persistent) {
            mTable.Clear(persistent);
        }

        public void ClearUserData() {
            mTable.ClearUserData();
        }

        public void SetPersist(string name, bool persist) {
            mTable.SetPersist(name, persist);
        }

        public void SnapshotSave() {
            mTable.SnapshotSave();
        }

        public void SnapshotRestore() {
            mTable.SnapshotRestore();
        }

        public void SnapshotDelete() {
            mTable.SnapshotDelete();
        }

        public MateSceneStateModule(SceneState.Table table) {
            mTable = table;
            mTable.onValueChange += OnValueChange;
        }

        private void OnValueChange(string name, SceneState.StateValue val) {
            if(valueChangeCallback != null) {
                DynValue luaVal;

                switch(val.type) {
                    case SceneState.Type.Integer:
                        luaVal = DynValue.NewNumber(val.ival);
                        break;
                    case SceneState.Type.Float:
                        luaVal = DynValue.NewNumber(val.fval);
                        break;
                    case SceneState.Type.String:
                        luaVal = DynValue.NewString(val.sval);
                        break;
                    default:
                        luaVal = DynValue.NewNil();
                        break;
                }

                valueChangeCallback(name, luaVal);
            }
        }

        private static bool _isTypeRegistered = false;
        public static void Register(Table table) {
            if(!_isTypeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<MateSceneStateModule>();
                _isTypeRegistered = true;
            }

            DynValue stateTableVal = table.Get(Const.luaMateSceneTable);
            if(stateTableVal.IsNil())
                table.Set(Const.luaMateSceneTable, stateTableVal = DynValue.NewTable(table.OwnerScript));

            Table stateTable = stateTableVal.Table;

            stateTable["Locals"] = new MateSceneStateModule(SceneState.instance.local);
            stateTable["Globals"] = new MateSceneStateModule(SceneState.instance.global);
        }
    }
}