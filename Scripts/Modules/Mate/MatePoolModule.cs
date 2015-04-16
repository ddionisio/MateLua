using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua.Modules {
    public struct MatePoolModule {
        private static bool _isTypeRegistered = false;
        public static void Register(Table table) {
            if(!_isTypeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<PoolController>();

                _isTypeRegistered = true;
            }

            table["Pool"] = typeof(PoolController);
        }
    }
}