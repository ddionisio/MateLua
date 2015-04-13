using UnityEngine;
using UnityEditor;

using System.Collections;

namespace M8.Lua {
    [CustomEditor(typeof(GlobalSettings))]
    public class GlobalSettingsInspector : Editor {
        public override void OnInspectorGUI() {
            GlobalSettings dat = target as GlobalSettings;

            dat.coreModules = (MoonSharp.Interpreter.CoreModules)EditorGUILayout.EnumMaskField("Core Modules", dat.coreModules);

            dat.unityCoreModules = (UnityCoreModules)EditorGUILayout.EnumMaskField("Unity Core Modules", dat.unityCoreModules);

            dat.mateCoreModules = (MateCoreModules)EditorGUILayout.EnumMaskField("Mate Core Modules", dat.mateCoreModules);

            dat.loader = EditorGUILayout.ObjectField("Script Loader", dat.loader, typeof(LoaderBase), false) as LoaderBase;
        }
    }
}