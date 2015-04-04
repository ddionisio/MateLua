using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua {
    public static class UnityCoreModuleRegister {
        public static Table RegisterUnityCoreModules(this Table table, UnityCoreModules modules) {
            //store all Unity related stuff to Unity table
            DynValue unityVal = DynValue.NewTable(table.OwnerScript);
            Table unityTable = unityVal.Table;

            table["Unity"] = unityVal;

            if(modules.Check(UnityCoreModules.Time)) unityTable.RegisterUnityTime();
            if(modules.Check(UnityCoreModules.Math)) unityTable.RegisterUnityMath();
            if(modules.Check(UnityCoreModules.Coroutine)) unityTable.RegisterUnityCoroutine();

            return table;
        }

        private static bool _isTimeRegistered = false;
        public static Table RegisterUnityTime(this Table table) {
            if(!_isTimeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<Time>();

                _isTimeRegistered = true;
            }
                        
            table["Time"] = typeof(Time);

            return table;
        }

        private static bool _isMathRegistered = false;
        public static Table RegisterUnityMath(this Table table) {
            if(!_isMathRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<Mathf>();
                                
                MoonSharp.Interpreter.UserData.RegisterType<Vector2>();
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Tuple, typeof(Vector2), itm => new Vector2(System.Convert.ToSingle(itm.Tuple[0].Number), System.Convert.ToSingle(itm.Tuple[1].Number)));
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Vector2), itm => new Vector2(System.Convert.ToSingle(itm.Table[1]), System.Convert.ToSingle(itm.Table[2])));

                MoonSharp.Interpreter.UserData.RegisterType<Vector3>();
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Tuple, typeof(Vector3), itm => new Vector3(System.Convert.ToSingle(itm.Tuple[0].Number), System.Convert.ToSingle(itm.Tuple[1].Number), System.Convert.ToSingle(itm.Tuple[2].Number)));
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Vector3), itm => new Vector3(System.Convert.ToSingle(itm.Table[1]), System.Convert.ToSingle(itm.Table[2]), System.Convert.ToSingle(itm.Table[3])));

                MoonSharp.Interpreter.UserData.RegisterType<Vector4>();
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Tuple, typeof(Vector4), itm => new Vector4(System.Convert.ToSingle(itm.Tuple[0].Number), System.Convert.ToSingle(itm.Tuple[1].Number), System.Convert.ToSingle(itm.Tuple[2].Number), System.Convert.ToSingle(itm.Tuple[3].Number)));
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Vector4), itm => new Vector4(System.Convert.ToSingle(itm.Table[1]), System.Convert.ToSingle(itm.Table[2]), System.Convert.ToSingle(itm.Table[3]), System.Convert.ToSingle(itm.Table[4])));

                MoonSharp.Interpreter.UserData.RegisterType<Quaternion>();
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Tuple, typeof(Quaternion), itm => new Quaternion(System.Convert.ToSingle(itm.Tuple[0].Number), System.Convert.ToSingle(itm.Tuple[1].Number), System.Convert.ToSingle(itm.Tuple[2].Number), System.Convert.ToSingle(itm.Tuple[3].Number)));
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Quaternion), itm => new Quaternion(System.Convert.ToSingle(itm.Table[1]), System.Convert.ToSingle(itm.Table[2]), System.Convert.ToSingle(itm.Table[3]), System.Convert.ToSingle(itm.Table[4])));

                MoonSharp.Interpreter.UserData.RegisterType<Matrix4x4>();

                _isMathRegistered = true;
            }

            table["Math"] = typeof(Mathf);
            table["Vector2"] = typeof(Vector2);
            table["Vector3"] = typeof(Vector3);
            table["Vector4"] = typeof(Vector4);
            table["Quaternion"] = typeof(Quaternion);
            table["Matrix4x4"] = typeof(Matrix4x4);

            return table;
        }

        private static bool _isCoroutineRegsitered = false;
        public static Table RegisterUnityCoroutine(this Table table) {
            if(!_isCoroutineRegsitered) {
                MoonSharp.Interpreter.UserData.RegisterType<YieldInstruction>();
                MoonSharp.Interpreter.UserData.RegisterType<UnityEngine.Coroutine>();

                _isCoroutineRegsitered = true;
            }

            table["WaitForFixedUpdate"] = (System.Func<YieldInstruction>)delegate() { return new WaitForFixedUpdate(); };
            table["WaitForEndOfFrame"] = (System.Func<YieldInstruction>)delegate() { return new WaitForEndOfFrame(); };
            table["WaitForSeconds"] = (System.Func<float, YieldInstruction>)delegate(float seconds) { return new WaitForSeconds(seconds); };

            return table;
        }
    }
}