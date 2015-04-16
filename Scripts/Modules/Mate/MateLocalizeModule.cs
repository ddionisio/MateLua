using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua.Modules {
    /// <summary>
    /// Note: for events, use .add and .remove (pauseCallback, sceneChangeCallback)
    /// </summary>
    public struct MateLocalizeModule {

        private static bool _isTypeRegistered = false;
        public static void Register(Table table) {
            if(!_isTypeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<Localize>();

                _isTypeRegistered = true;
            }

            table["Localize"] = Localize.instance;
        }
    }
}