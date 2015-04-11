using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8.Lua {
    [CustomEditor(typeof(LuaBehaviour))]
    public class LuaBehaviourInspector : Editor {
        private bool mVarFoldout = true;

        SerializedProperty loaderOverride;
        SerializedProperty scriptFrom;
        SerializedProperty scriptPath;
        SerializedProperty scriptText;
        SerializedProperty loadOnAwake;
        SerializedProperty properties;

        void OnEnable() {
            loaderOverride = serializedObject.FindProperty("loaderOverride");
            scriptFrom = serializedObject.FindProperty("scriptFrom");
            scriptPath = serializedObject.FindProperty("scriptPath");
            scriptText = serializedObject.FindProperty("scriptText");
            loadOnAwake = serializedObject.FindProperty("loadOnAwake");
            properties = serializedObject.FindProperty("properties");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(loaderOverride, new GUIContent("Loader Override"));

            GUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(scriptFrom);

            LuaBehaviour.LoadFrom curScriptLoadFrom = (LuaBehaviour.LoadFrom)scriptFrom.enumValueIndex;

            switch(curScriptLoadFrom) {
                case LuaBehaviour.LoadFrom.File:
                    EditorGUILayout.PropertyField(scriptPath);
                    EditorGUILayout.PropertyField(loadOnAwake);
                    break;
                case LuaBehaviour.LoadFrom.TextAsset:
                    EditorGUILayout.PropertyField(scriptText);
                    EditorGUILayout.PropertyField(loadOnAwake);
                    break;
            }
                        
            GUILayout.EndVertical();

            M8.EditorExt.Utility.DrawSeparator();

            mVarFoldout = EditorGUILayout.Foldout(mVarFoldout, "Properties");
            if(mVarFoldout) {
                for(int i = 0; i < properties.arraySize; i++) {
                    var elem = properties.GetArrayElementAtIndex(i);

                    GUILayout.BeginHorizontal();

                    var _type = elem.FindPropertyRelative("type");

                    if(EditorGUILayout.PropertyField(_type, new GUIContent(), GUILayout.Width(90f))) {
                        elem.Reset();
                    }

                    EditorGUILayout.PropertyField(elem.FindPropertyRelative("name"), new GUIContent(), GUILayout.Width(100f));

                    EditorGUIUtility.labelWidth = 16f;

                    switch((LuaBehaviour.Variable.Type)_type.enumValueIndex) {
                        case LuaBehaviour.Variable.Type.Boolean:
                            elem.FindPropertyRelative("iVal").intValue = EditorGUILayout.Toggle(elem.FindPropertyRelative("iVal").intValue > 0) ? 1 : 0;
                            break;
                        case LuaBehaviour.Variable.Type.Integer:
                            EditorGUILayout.PropertyField(elem.FindPropertyRelative("iVal"), new GUIContent());
                            break;
                        case LuaBehaviour.Variable.Type.Float:
                            EditorGUILayout.PropertyField(elem.FindPropertyRelative("fVal"), new GUIContent());
                            break;
                        case LuaBehaviour.Variable.Type.String:
                            EditorGUILayout.PropertyField(elem.FindPropertyRelative("sVal"), new GUIContent());
                            break;
                        case LuaBehaviour.Variable.Type.GameObject:
                            EditorGUILayout.PropertyField(elem.FindPropertyRelative("goVal"), new GUIContent());
                            break;
                    }

                    EditorGUIUtility.labelWidth = 0f;

                    if(GUILayout.Button("+", GUILayout.Width(24f))) {
                        properties.InsertArrayElementAtIndex(i);
                        break;
                    }
                    if(GUILayout.Button("-", GUILayout.Width(24f))) {
                        properties.DeleteArrayElementAtIndex(i);
                        break;
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(6f);

                if(GUILayout.Button("New")) {
                    properties.InsertArrayElementAtIndex(properties.arraySize);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}