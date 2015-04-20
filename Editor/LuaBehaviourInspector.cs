using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8.Lua {
    [CustomEditor(typeof(LuaBehaviour))]
    public class LuaBehaviourInspector : Editor {
        private bool mVarFoldout = true;

        SerializedProperty _name;
        SerializedProperty loaderOverride;
        SerializedProperty scriptFrom;
        SerializedProperty scriptPath;
        SerializedProperty scriptText;
        SerializedProperty loadOnAwake;
        SerializedProperty properties;

        void OnEnable() {
            _name = serializedObject.FindProperty("name");
            loaderOverride = serializedObject.FindProperty("loaderOverride");
            scriptFrom = serializedObject.FindProperty("scriptFrom");
            scriptPath = serializedObject.FindProperty("scriptPath");
            scriptText = serializedObject.FindProperty("scriptText");
            loadOnAwake = serializedObject.FindProperty("loadOnAwake");
            properties = serializedObject.FindProperty("properties");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_name);

            EditorGUILayout.PropertyField(loaderOverride);

            GUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(scriptFrom);

            LuaBehaviour.LoadFrom curScriptLoadFrom = (LuaBehaviour.LoadFrom)scriptFrom.enumValueIndex;

            switch(curScriptLoadFrom) {
                case LuaBehaviour.LoadFrom.File:
                    EditorGUILayout.PropertyField(scriptPath);
                    EditorGUILayout.PropertyField(loadOnAwake);

                    scriptText.objectReferenceValue = null;
                    break;
                case LuaBehaviour.LoadFrom.TextAsset:
                    EditorGUILayout.PropertyField(scriptText);
                    EditorGUILayout.PropertyField(loadOnAwake);

                    scriptPath.stringValue = "";
                    break;

                case LuaBehaviour.LoadFrom.String:
                    scriptText.objectReferenceValue = null;
                    scriptPath.stringValue = "";
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

                    Vector4 vVal = elem.FindPropertyRelative("vVal").vector4Value;

                    switch((SerializedVariable.Type)_type.enumValueIndex) {
                        case SerializedVariable.Type.Boolean:
                            elem.FindPropertyRelative("iVal").intValue = EditorGUILayout.Toggle(":", elem.FindPropertyRelative("iVal").intValue > 0) ? 1 : 0;
                            break;
                        case SerializedVariable.Type.Integer:
                            EditorGUILayout.PropertyField(elem.FindPropertyRelative("iVal"), new GUIContent(":"));
                            break;
                        case SerializedVariable.Type.Float:
                            vVal.x = EditorGUILayout.FloatField(":", vVal.x);
                            break;
                        case SerializedVariable.Type.Vector2:
                            Vector2 v2 = EditorGUILayout.Vector2Field(":", new Vector2(vVal.x, vVal.y));
                            vVal.x = v2.x; vVal.y = v2.y;
                            break;
                        case SerializedVariable.Type.Vector3:
                            Vector3 v3 = EditorGUILayout.Vector3Field(":", new Vector3(vVal.x, vVal.y, vVal.z));
                            vVal.x = v3.x; vVal.y = v3.y; vVal.z = v3.z;
                            break;
                        case SerializedVariable.Type.Vector4:
                            vVal = EditorGUILayout.Vector4Field(":", vVal);
                            break;
                        case SerializedVariable.Type.Rotation:
                            Vector3 r = EditorGUILayout.Vector3Field(":", new Vector3(vVal.x, vVal.y, vVal.z));
                            vVal.x = r.x; vVal.y = r.y; vVal.z = r.z;
                            break;
                        case SerializedVariable.Type.String:
                            EditorGUILayout.PropertyField(elem.FindPropertyRelative("sVal"), new GUIContent(":"));
                            break;
                        case SerializedVariable.Type.GameObject:
                            EditorGUILayout.PropertyField(elem.FindPropertyRelative("goVal"), new GUIContent(":"));
                            break;
                    }

                    elem.FindPropertyRelative("vVal").vector4Value = vVal;

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

            //runtime controls
            if(Application.isPlaying) {
                LuaBehaviour dat = target as LuaBehaviour;

                M8.EditorExt.Utility.DrawSeparator();

                //load
                if(GUILayout.Button(dat.isLoaded ? "Reload" : "Load")) {
                    dat.Load();
                }
            }
        }
    }
}