using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8.Lua {
    public class LuaScriptsBrowser : EditorWindow {
        public delegate void OnSelect(LuaScriptsBrowser dlg, string path);

        public event OnSelect selectCallback;

        private string[] mPaths;
        private string[] mFullpaths;
        private int mCurInd;

        public static LuaScriptsBrowser Open() {
            return EditorWindow.GetWindow(typeof(LuaScriptsBrowser)) as LuaScriptsBrowser;
        }

        public void Select(string path) {
            if(mPaths != null) {
                for(int i = 0; i < mPaths.Length; i++) {
                    if(mPaths[i] == path) {
                        mCurInd = i;
                        break;
                    }
                }
            }
        }

        void OnEnable() {
            //grab full paths of all lua files
            List<string> luaPaths = new List<string>();
            foreach(string luaPath in new LuaScriptsProcess.AssetPathEnumerator()) {
                luaPaths.Add(luaPath);
            }

            mFullpaths = luaPaths.ToArray();

            //generate relative paths
            string scriptDir = EditorPrefs.GetString(LuaScriptsProcess.scriptDirKey, LuaScriptsProcess.defaultScriptDir);

            mPaths = new string[mFullpaths.Length];

            for(int i = 0; i < mFullpaths.Length; i++) {
                int subInd = mFullpaths[i].IndexOf(scriptDir);
                if(subInd != -1)
                    subInd = subInd+scriptDir.Length+1;
                else
                    subInd = mFullpaths[i].LastIndexOf('/');

                mPaths[i] = mFullpaths[i].Substring(subInd, mFullpaths[i].Length - subInd - 4);
            }
        }

        void OnDestroy() {
            selectCallback = null;
        }

        void OnGUI() {
            GUILayout.Label("Scripts");

            GUILayout.BeginHorizontal();

            int newInd = EditorGUILayout.Popup(mCurInd, mPaths);
            mCurInd = Mathf.Clamp(newInd, 0, mPaths.Length-1);
            
            //external edit
            string externEditPath = EditorPrefs.GetString(LuaScriptsProcess.externalEditorKey, "");

            GUI.enabled = !string.IsNullOrEmpty(externEditPath);

            if(M8.EditorExt.Utility.DrawSimpleButton("E", "Edit External")) {
                try {
                    string args = EditorPrefs.GetString(LuaScriptsProcess.externalEditorArgFormatKey, "{0}");
                    System.Diagnostics.Process.Start(externEditPath, string.Format(args, string.Format("\"{0}\"", mFullpaths[mCurInd])));
                }
                catch(System.Exception e) {
                    Debug.LogError(e);
                }
            }

            GUI.enabled = true;
            //

            if(M8.EditorExt.Utility.DrawSimpleButton("o", "Ping Location")) {
                string dataPath = Application.dataPath;

                int _i = mFullpaths[mCurInd].IndexOf(dataPath);

                string _p = _i != -1 ? mFullpaths[mCurInd].Substring(_i + dataPath.Length + 1) : mFullpaths[mCurInd];

                Object obj = AssetDatabase.LoadMainAssetAtPath("Assets/"+_p);
                if(obj)
                    EditorGUIUtility.PingObject(obj);
            }

            GUILayout.EndHorizontal();

            //display full path
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label(mFullpaths[mCurInd]);

            GUILayout.EndVertical();

            M8.EditorExt.Utility.DrawSeparator();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if(GUILayout.Button("Select", GUILayout.MaxWidth(150f))) {
                if(selectCallback != null)
                    selectCallback(this, mPaths[mCurInd]);

                Close();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }
    }
}