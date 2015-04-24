using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MoonSharp.Interpreter;

namespace M8.Lua {
    /// <summary>
    /// Load using Unity's Resources API, note that all paths are relative to Assets/Resources.
    /// </summary>
    public class LoaderFromResources : LoaderBase {
        private Dictionary<string, string> mResources = new Dictionary<string, string>();

        public override bool ScriptFileExists(string file) {
            file = GetPath(file);

            return LoadAsset(file) != null;
        }

        public override object LoadFile(string file, Table globalContext) {
            file = GetPath(file);

            string ret = LoadAsset(file);
            if(ret == null) {
                Debug.LogError(string.Format(
@"Cannot load script '{0}'. By default, scripts should be .txt files placed under a Assets/Resources/{1} directory.
If you want scripts to be put in another directory or another way, use a custom instance of UnityAssetsScriptLoader or implement
your own IScriptLoader (possibly extending ScriptLoaderBase).", file, rootDir));
            }

            return ret;
        }

        public override void ClearCache(string file) {
            file = GetPath(file);
            mResources.Remove(file);
        }

        private string LoadAsset(string path) {
            string ret = null;

            if(!mResources.TryGetValue(path, out ret)) {
                var asset = Resources.Load<TextAsset>(path);
                if(asset)
                    mResources.Add(path, ret = asset.text);
                else
                    mResources.Add(path, null);
            }

            return ret;
        }

        private string GetPath(string filename) {
            var sb = new System.Text.StringBuilder(rootDir);

            sb.Append('/');

            int extInd = filename.LastIndexOf('.');

            if(extInd != -1) //remove extension
                sb.Append(filename.Substring(0, extInd));
            else
                sb.Append(filename);
                        
            return sb.ToString();
        }
    }
}