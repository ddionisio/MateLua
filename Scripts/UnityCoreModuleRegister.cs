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

            return table;
        }

        public static Table RegisterUnityTime(this Table table) {
            MoonSharp.Interpreter.UserData.RegisterType<Time>();
                        
            table["Time"] = typeof(Time);

            return table;
        }

        private static DynValue mMathModule = null;
        private static bool mIsMathModuleInit = false;
        public static Table RegisterUnityMath(this Table table) {
            if(!mIsMathModuleInit) {
                mMathModule = DynValue.NewTable(table.OwnerScript);
                Table mathTable = mMathModule.Table;

                mathTable["Deg2Rad"] = Mathf.Deg2Rad;
                mathTable["Epsilon"] = Mathf.Epsilon;
                mathTable["Infinity"] = Mathf.Infinity;
                mathTable["NegativeInfinity"] = Mathf.NegativeInfinity;
                mathTable["PI"] = Mathf.PI;
                mathTable["Rad2Deg"] = Mathf.Rad2Deg;

                mathTable["Abs"] = (System.Func<float, float>)Mathf.Abs;
                mathTable["Acos"] = (System.Func<float, float>)Mathf.Acos;
                mathTable["Approximately"] = (System.Func<float, float, bool>)Mathf.Approximately;
                mathTable["Asin"] = (System.Func<float, float>)Mathf.Asin;
                mathTable["Atan"] = (System.Func<float, float>)Mathf.Atan;
                mathTable["Atan2"] = (System.Func<float, float, float>)Mathf.Atan2;
                mathTable["Ceil"] = (System.Func<float, float>)Mathf.Ceil;
                mathTable["Clamp"] = (System.Func<float, float, float, float>)Mathf.Clamp;
                mathTable["Clamp01"] = (System.Func<float, float>)Mathf.Clamp01;
                mathTable["ClosestPowerOfTwo"] = (System.Func<int, int>)Mathf.ClosestPowerOfTwo;
                mathTable["Cos"] = (System.Func<float, float>)Mathf.Cos;
                mathTable["DeltaAngle"] = (System.Func<float, float, float>)Mathf.DeltaAngle;
                mathTable["Exp"] = (System.Func<float, float>)Mathf.Exp;
                mathTable["Floor"] = (System.Func<float, float>)Mathf.Floor;
                mathTable["Gamma"] = (System.Func<float, float, float, float>)Mathf.Gamma;
                mathTable["GammaToLinearSpace"] = (System.Func<float, float>)Mathf.GammaToLinearSpace;
                mathTable["InverseLerp"] = (System.Func<float, float, float, float>)Mathf.InverseLerp;
                mathTable["IsPowerOfTwo"] = (System.Func<int, bool>)Mathf.IsPowerOfTwo;
                mathTable["Lerp"] = (System.Func<float, float, float, float>)Mathf.Lerp;
                mathTable["LerpAngle"] = (System.Func<float, float, float, float>)Mathf.LerpAngle;
                mathTable["LinearToGammaSpace"] = (System.Func<float, float>)Mathf.LinearToGammaSpace;
                mathTable["Log"] = (System.Func<float, float>)Mathf.Log;
                mathTable["LogN"] = (System.Func<float, float, float>)Mathf.Log;
                mathTable["Log10"] = (System.Func<float, float>)Mathf.Log10;
                mathTable["Max"] = (System.Func<float, float, float>)Mathf.Max;
                mathTable["MaxN"] = (System.Func<float[], float>)Mathf.Max;
                mathTable["Min"] = (System.Func<float, float, float>)Mathf.Min;
                mathTable["MinN"] = (System.Func<float[], float>)Mathf.Min;
                mathTable["MoveTowards"] = (System.Func<float, float, float, float>)Mathf.MoveTowards;
                mathTable["MoveTowardsAngle"] = (System.Func<float, float, float, float>)Mathf.MoveTowardsAngle;
                mathTable["NextPowerOfTwo"] = (System.Func<int, int>)Mathf.NextPowerOfTwo;
                mathTable["PerlinNoise"] = (System.Func<float, float, float>)Mathf.PerlinNoise;
                mathTable["PingPong"] = (System.Func<float, float, float>)Mathf.PingPong;
                mathTable["Pow"] = (System.Func<float, float, float>)Mathf.Pow;
                mathTable["Repeat"] = (System.Func<float, float, float>)Mathf.Repeat;
                mathTable["Round"] = (System.Func<float, float>)Mathf.Round;
                mathTable["Sign"] = (System.Func<float, float>)Mathf.Sign;
                mathTable["Sin"] = (System.Func<float, float>)Mathf.Sin;
                mathTable["Sqrt"] = (System.Func<float, float>)Mathf.Sqrt;
                mathTable["Tan"] = (System.Func<float, float>)Mathf.Tan;

                //TODO: smooth step

                //common types
                //MoonSharp.Interpreter.UserData.RegisterType<Matrix4x4>();

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
                                
                mIsMathModuleInit = true;
            }

            table["Math"] = mMathModule;

            table["Vector3"] = typeof(Vector3);
            table["Quaternion"] = typeof(Quaternion);
            //table["Matrix4x4"] = typeof(Matrix4x4);

            return table;
        }
    }
}