using UnityEngine;
using UnityEditor;

using System.IO;
using System.Collections;

namespace M8.Lua {
    public class LuaScriptsEditor : EditorWindow {
        private bool mActive;
        private string mScriptDir;
        private string mDestDir;

        [MenuItem("M8/Lua")]
        static void DoIt() {
            EditorWindow.GetWindow(typeof(LuaScriptsEditor));
        }

        void OnEnable() {
            mActive = EditorPrefs.GetBool(LuaScriptsProcess.activeKey, LuaScriptsProcess.defaultActive);
            mScriptDir = EditorPrefs.GetString(LuaScriptsProcess.scriptDirKey, LuaScriptsProcess.defaultScriptDir);
            mDestDir = EditorPrefs.GetString(LuaScriptsProcess.destDirKey, LuaScriptsProcess.defaultDestDir);
        }

        // Use this for initialization
        void OnGUI() {
            GUILayout.Label("Process Settings");

            GUILayout.Label("The process settings determine where lua files reside whenever assets are being processed by Unity.  Activate this for when you want to load lua files via Resources.  Any lua files created from the Script Dir will be copied over to the Dest Dir as text files.", GUI.skin.textArea);

            GUILayout.Space(4);

            bool a = EditorGUILayout.Toggle("Active", mActive);
            if(mActive != a)
                EditorPrefs.SetBool(LuaScriptsProcess.activeKey, mActive = a);

            string sd = EditorGUILayout.TextField(new GUIContent("Script Dir", "This is the base directory for the lua files.  You can have multiple folders with this name anywhere in the project, all of them will be condensed into the Dest Dir."), mScriptDir);
            if(mScriptDir != sd)
                EditorPrefs.SetString(LuaScriptsProcess.scriptDirKey, mScriptDir = sd);

            string dd = EditorGUILayout.TextField(new GUIContent("Dest Dir", "This is the destination directory for the lua files.  Normally you want them to be in Resources and its own root directory.  You can modify this depending on how you setup the Lua interpreter."), mDestDir);
            if(mDestDir != dd)
                EditorPrefs.SetString(LuaScriptsProcess.destDirKey, mDestDir = dd);

            M8.Editor.Utility.DrawSeparator();

            if(GUILayout.Button(new GUIContent("Process All Scripts", "Searches for all lua files in the Asset folder based on Script Dir, and then copy them all to Dest Dir as txt file."))) {
                string[] paths = Directory.GetFiles(Application.dataPath, "*.lua", SearchOption.AllDirectories);
                for(int i = 0; i < paths.Length; i++) {
                    if(paths[i].Contains(mScriptDir)) {
                        string path = paths[i].Replace('\\', '/');
                        LuaScriptsProcess.GenerateTextFromLuaFile(mScriptDir, mDestDir, path);
                    }
                }

                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
        }
    }
}