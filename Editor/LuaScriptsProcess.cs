using UnityEngine;
using UnityEditor;

using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace M8.Lua {
    public class LuaScriptsProcess : AssetPostprocessor {
        public const bool defaultActive = true;
        public const string defaultScriptDir = "LuaScript"; //directory to check for lua scripts, put your lua files in any LuaScript directories to generate them.
        public const string defaultDestDir = "Assets/Resources/Lua"; //directory to generate text files for lua script for loading via Resources

        public const string externalEditorKey = "m8lua_external";
        public const string externalEditorArgFormatKey = "m8lua_external_args";

        public class AssetPathEnumerator : IEnumerable {
            public IEnumerator GetEnumerator() {
                string scriptDir = EditorPrefs.GetString(LuaScriptsProcess.scriptDirKey, LuaScriptsProcess.defaultScriptDir);

                string[] dirs = Directory.GetDirectories(Application.dataPath, "*"+scriptDir, SearchOption.AllDirectories);
                for(int d = 0; d < dirs.Length; d++) {
                    string[] luaFilePaths = Directory.GetFiles(dirs[d], "*.lua", SearchOption.AllDirectories);
                    for(int i = 0; i < luaFilePaths.Length; i++)
                        yield return luaFilePaths[i].Replace('\\', '/');
                }
            }
        }

        public static string activeKey { get { return M8.EditorExt.Utility.PreferenceKey("lua", "active"); } }
        public static string scriptDirKey { get { return M8.EditorExt.Utility.PreferenceKey("lua", "scriptDir"); } }
        public static string destDirKey { get { return M8.EditorExt.Utility.PreferenceKey("lua", "destDir"); } }
        

        private static string GetDestPath(string scriptDir, string destDir, string path) {
            int luaScriptDirInd = path.IndexOf(scriptDir);
            if(luaScriptDirInd != -1) {
                int splitInd = luaScriptDirInd+scriptDir.Length;
                int subInd = splitInd + 1, subLen = path.Length - splitInd - 4;
                if(subInd < path.Length && subLen > 0)
                    return destDir + '/' + path.Substring(splitInd + 1, path.Length - splitInd - 4) + "bytes";
                else
                    Debug.Log("Unable to process: "+path);
            }

            return "";
        }

        public static bool GenerateTextFromLuaFile(string scriptDir, string destDir, string path) {
            if(path.EndsWith(".lua")) {
                if(path.Contains("Resources") || path.Contains("StreamingAssets")) //there shouldn't be lua files in Resources anyhow...
                    return false;

                string destPath = GetDestPath(scriptDir, destDir, path);
                if(!string.IsNullOrEmpty(destPath)) {
                    //create directory
                    int slashInd = destPath.LastIndexOf('/');
                    if(slashInd != -1) {
                        string d = destPath.Substring(0, slashInd);
                        if(!Directory.Exists(d))
                            Directory.CreateDirectory(d);
                    }

                    File.Copy(path, destPath, true);
                    return true;
                }
            }

            return false;
        }

        private static bool DeleteAssociatedTextFile(string scriptDir, string destDir, string path) {
            if(path.EndsWith(".lua")) {
                if(path.Contains("Resources") || path.Contains("StreamingAssets")) //there shouldn't be lua files in Resources anyhow...
                    return false;

                string destPath = GetDestPath(scriptDir, destDir, path);
                if(!string.IsNullOrEmpty(destPath)) {
                    File.Delete(destPath);
                    File.Delete(destPath+".meta");

                    //delete directory if empty
                    string d = destPath.Substring(0, destPath.LastIndexOf('/'));
                    if(Directory.GetFiles(d).Length == 0) {
                        Directory.Delete(d);
                        File.Delete(d+".meta");
                    }

                    return true;
                }
            }

            return false;
        }

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            if(!EditorPrefs.GetBool(activeKey, defaultActive)) return;

            bool refreshAssets = false;

            string scriptDir = EditorPrefs.GetString(scriptDirKey, defaultScriptDir);
            string destDir = EditorPrefs.GetString(destDirKey, defaultDestDir);

            for(int i = 0; i < importedAssets.Length; i++) {
                if(GenerateTextFromLuaFile(scriptDir, destDir, importedAssets[i]))
                    refreshAssets = true;
            }

            for(int i = 0; i < deletedAssets.Length; i++) {
                if(DeleteAssociatedTextFile(scriptDir, destDir, deletedAssets[i]))
                    refreshAssets = true;
            }

            for(int i = 0; i < movedAssets.Length; i++) {
                if(GenerateTextFromLuaFile(scriptDir, destDir, movedAssets[i]))
                    refreshAssets = true;
            }

            for(int i = 0; i < movedFromAssetPaths.Length; i++) {
                if(DeleteAssociatedTextFile(scriptDir, destDir, movedFromAssetPaths[i]))
                    refreshAssets = true;
            }

            if(refreshAssets)
                AssetDatabase.Refresh(ImportAssetOptions.Default);
        }

        

        /// <summary>
        /// Grab the .lua full file path from the given path relative to "Script Dir" from Lua Configuration (NOTE: will use the first match found from search)
        /// </summary>
        public static string GetSourcePath(string path) {
            string searchPath = path + ".lua";

            foreach(string luaPath in new AssetPathEnumerator()) {
                if(luaPath.Contains(searchPath))
                    return luaPath;
            }

            return "";
        }

        public static void PingSourcePath(string path) {
            if(string.IsNullOrEmpty(path)) return;

            string dataPath = Application.dataPath;

            string searchPath = path + ".lua";

            foreach(string luaPath in new AssetPathEnumerator()) {
                if(luaPath.Contains(searchPath)) {
                    string _p;

                    int _i = luaPath.IndexOf(dataPath);

                    _p = _i != -1 ? luaPath.Substring(_i + dataPath.Length + 1) : luaPath;

                    Object obj = AssetDatabase.LoadMainAssetAtPath("Assets/"+_p);
                    if(obj)
                        EditorGUIUtility.PingObject(obj);
                }
            }
        }
    }
}