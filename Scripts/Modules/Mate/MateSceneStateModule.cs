using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MoonSharp.Interpreter;

namespace M8.Lua.Modules {
    public class MateSceneStateModule {
        private SceneState.Table mTable;
        private Script mScript;
        private Dictionary<DynValue, SceneState.StateCallback> mBinds;

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

        /// <summary>
        /// first param is func, the rest is whatever
        /// </summary>
        /// <param name="args"></param>
        public void AddValueChangeCall(CallbackArguments args) {
            DynValue luaFunc = args[0];

            //setup parameters, first element will be name, second is the value, the rest is whatever
            DynValue[] parms = new DynValue[args.Count + 1];
            System.Array.Copy(args.GetArray(1), 0, parms, 2, args.Count - 1);

            if(luaFunc.Type == DataType.String)
                luaFunc = mScript.Globals.Get(luaFunc);

            SceneState.StateCallback func = delegate(string name, SceneState.StateValue val) {
                parms[0] = DynValue.NewString(name);
                
                switch(val.type) {
                    case SceneState.Type.Integer:
                        parms[1] = DynValue.NewNumber(val.ival);
                        break;
                    case SceneState.Type.Float:
                        parms[1] = DynValue.NewNumber(val.fval);
                        break;
                    case SceneState.Type.String:
                        parms[1] = DynValue.NewString(val.sval);
                        break;
                    default:
                        parms[1] = DynValue.NewNil();
                        break;
                }

                try {
                    mScript.Call(luaFunc, parms);
                }
                catch(InterpreterException ie) {
                    Debug.LogError(ie.DecoratedMessage);
                }
            };

            if(mBinds.ContainsKey(luaFunc)) {
                mTable.onValueChange -= mBinds[luaFunc];
                mBinds[luaFunc] = func;
            }
            else
                mBinds.Add(luaFunc, func);

            mTable.onValueChange += func;
        }

        public void RemoveValueChangeCall(DynValue luaFunc) {
            if(luaFunc.Type == DataType.String)
                luaFunc = mScript.Globals.Get(luaFunc);

            SceneState.StateCallback func;
            if(mBinds.TryGetValue(luaFunc, out func)) {
                mTable.onValueChange -= func;
                mBinds.Remove(luaFunc);
            }
        }

        public void ClearValueChangeCalls() {
            foreach(var pair in mBinds)
                mTable.onValueChange -= pair.Value;
            mBinds.Clear();
        }

        public MateSceneStateModule(Script script, SceneState.Table table) {
            mScript = script;
            mTable = table;
            mBinds = new Dictionary<DynValue, SceneState.StateCallback>();
        }

        private static bool _isTypeRegistered = false;
        public static void Register(Table table) {
            if(!_isTypeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<MateSceneStateModule>();
                _isTypeRegistered = true;
            }

            DynValue stateTableVal = table.Get("Scene");
            if(stateTableVal.IsNil())
                table.Set("Scene", stateTableVal = DynValue.NewTable(table.OwnerScript));

            Table stateTable = stateTableVal.Table;

            stateTable["Locals"] = new MateSceneStateModule(table.OwnerScript, SceneState.instance.local);
            stateTable["Globals"] = new MateSceneStateModule(table.OwnerScript, SceneState.instance.global);
        }
    }
}