using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8.Lua {
    [CustomEditor(typeof(LuaBehaviour))]
    public class LuaBehaviourInspector : UnityEditor.Editor {
        private bool mVarFoldout;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            M8.Editor.Utility.DrawSeparator();

            mVarFoldout = EditorGUILayout.Foldout(mVarFoldout, "Variables");
            if(mVarFoldout) {
                LuaBehaviour dat = target as LuaBehaviour;
                                
                for(int i = 0; i < dat.initialVars.Length; i++) {
                    LuaBehaviour.Variable var = dat.initialVars[i];

                    GUILayout.BeginHorizontal();

                    LuaBehaviour.Variable.Type t = (LuaBehaviour.Variable.Type)EditorGUILayout.EnumPopup(var.type, GUILayout.Width(80f));
                    if(var.type != t) {
                        var.type = t;
                        var.Reset();
                    }

                    var.name = EditorGUILayout.TextField(var.name, GUILayout.Width(100f));

                    EditorGUIUtility.labelWidth = 16f;

                    switch(var.type) {
                        case LuaBehaviour.Variable.Type.Boolean:
                            var.iVal = EditorGUILayout.Toggle(":", var.iVal != 0) ? 1 : 0;
                            break;
                        case LuaBehaviour.Variable.Type.Integer:
                            var.iVal = EditorGUILayout.IntField(":", var.iVal);
                            break;
                        case LuaBehaviour.Variable.Type.Float:
                            var.fVal = EditorGUILayout.FloatField(":", var.fVal);
                            break;
                        case LuaBehaviour.Variable.Type.String:
                            var.sVal = EditorGUILayout.TextField(":", var.sVal);
                            break;
                        case LuaBehaviour.Variable.Type.Object:
                            var.oVal = EditorGUILayout.ObjectField(":", var.oVal, typeof(Object), true);
                            break;
                    }

                    EditorGUIUtility.labelWidth = 0f;

                    if(GUILayout.Button("+", GUILayout.Width(24f))) {
                        M8.ArrayUtil.InsertAfter(ref dat.initialVars, i, var);
                        return;
                    }
                    if(GUILayout.Button("-", GUILayout.Width(24f))) {
                        M8.ArrayUtil.RemoveAt(ref dat.initialVars, i);
                        return;
                    }

                    GUILayout.EndHorizontal();

                    dat.initialVars[i] = var;
                }

                GUILayout.Space(6f);

                if(GUILayout.Button("New")) {
                    System.Array.Resize(ref dat.initialVars, dat.initialVars.Length+1);
                    GUI.changed = true;
                }

                if(GUI.changed)
                    EditorUtility.SetDirty(target);
            }
        }
    }
}