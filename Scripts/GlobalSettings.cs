using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua {
    /// <summary>
    /// Initialize and setup Moonsharp.  Add this to Resources/core.prefab
    /// </summary>
    [PrefabCore]
    [AddComponentMenu("M8/Lua/Global Settings")]
    public class GlobalSettings : SingletonBehaviour<GlobalSettings> {
        public const CoreModules coreModuleDefault = CoreModules.Basic | CoreModules.Bit32 | CoreModules.Coroutine | CoreModules.Dynamic | CoreModules.ErrorHandling | CoreModules.LoadMethods | CoreModules.Math | CoreModules.Metatables | CoreModules.OS_Time | CoreModules.String | CoreModules.Table | CoreModules.TableIterators;
        public const UnityCoreModules unityCoreModuleDefault = UnityCoreModules.Time | UnityCoreModules.Math | UnityCoreModules.Coroutine;

        public CoreModules coreModules = coreModuleDefault;
        public UnityCoreModules unityCoreModules = unityCoreModuleDefault;
        public LoaderBase loader;

        protected override void OnInstanceInit() {
            if(loader)
                Script.DefaultOptions.ScriptLoader = loader;

            Script.DefaultOptions.DebugPrint = Debug.Log;
        }
    }
}