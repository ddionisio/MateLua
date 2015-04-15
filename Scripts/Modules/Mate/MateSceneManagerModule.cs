using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua.Modules {
    /// <summary>
    /// Note: for events, use .add and .remove (pauseCallback, sceneChangeCallback)
    /// </summary>
    public struct MateSceneManagerModule {

        private static bool _isTypeRegistered = false;
        public static void Register(Table table) {
            if(!_isTypeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<SceneManager>();

                _isTypeRegistered = true;
            }

            DynValue stateTableVal = table.Get(Const.luaMateSceneTable);
            if(stateTableVal.IsNil())
                table.Set(Const.luaMateSceneTable, stateTableVal = DynValue.NewTable(table.OwnerScript));

            Table stateTable = stateTableVal.Table;

            stateTable["Manager"] = SceneManager.instance;
        }
    }
}