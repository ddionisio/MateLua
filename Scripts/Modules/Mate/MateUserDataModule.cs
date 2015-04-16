using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua.Modules {
    public struct MateUserDataModule {
        private UserData mDat;
                
        public static DynValue main {
            get {
                DynValue ret;

                UserData ud = UserData.main;
                if(ud)
                    ret = MoonSharp.Interpreter.UserData.Create(new MateUserDataModule(ud));
                else
                    ret = DynValue.Nil;

                return ret;
            }
        }

        public static DynValue Get(string index) {
            DynValue ret;

            UserData ud = UserData.GetInstance(index);
            if(ud)
                ret = MoonSharp.Interpreter.UserData.Create(new MateUserDataModule(ud));
            else
                ret = DynValue.Nil;

            return ret;
        }

        public DynValue this[string index] {
            get {
                DynValue ret;

                System.Type type = mDat.GetType(index);
                if(type == typeof(int))
                    ret = DynValue.NewNumber(mDat.GetInt(index));
                else if(type == typeof(float))
                    ret = DynValue.NewNumber(mDat.GetFloat(index));
                else if(type == typeof(string))
                    ret = DynValue.NewString(mDat.GetString(index));
                else
                    ret = DynValue.Nil;

                return ret;
            }

            set {
                switch(value.Type) {
                    case DataType.Number:
                        mDat.SetFloat(index, System.Convert.ToSingle(value.Number));
                        break;
                    case DataType.String:
                        mDat.SetString(index, value.String);
                        break;
                }
            }
        }

        public bool started { get { return mDat.started; } }
        public int count { get { return mDat.valueCount; } }

        public void SetInt(string index, int val) {
            mDat.SetInt(index, val);
        }

        public void SnapshotSave() {
            mDat.SnapshotSave();
        }

        public void SnapshotRestore() {
            mDat.SnapshotRestore();
        }

        public void SnapshotDelete() {
            mDat.SnapshotDelete();
        }

        public void SnapshotPreserve(string key) {
            mDat.SnapshotPreserve(key);
        }

        public void Load() {
            mDat.Load();
        }

        public void Save() {
            mDat.Save();
        }

        public void Delete(string key) {
            mDat.Delete(key);
        }

        public void DeleteAll() {
            mDat.Delete();
        }

        public void DeleteAllByNameContain(string nameContains) {
            mDat.DeleteAllByNameContain(nameContains);
        }

        public MateUserDataModule(UserData dat) {
            mDat = dat;
        }

        private static bool _isTypeRegistered = false;
        public static void Register(Table table) {
            if(!_isTypeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<MateUserDataModule>();

                _isTypeRegistered = true;
            }

            table["UserData"] = typeof(MateUserDataModule);
        }
    }
}