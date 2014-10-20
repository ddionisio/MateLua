using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua {
    /// <summary>
    /// Use this to setup the Lua VM, such as File Manager, Debug, etc.
    /// Ideally you want this on a core object that persists across scenes.
    /// </summary>
    [AddComponentMenu("M8/Lua/Settings")]
    public class LuaSettings : MonoBehaviour {
        public enum RootType {
            Resource,
            LocalStream
        }

        public RootType root = RootType.Resource;
        public string rootPath = LuaFileManager.defaultBasePath;

        void Awake() {
            switch(root) {
                case RootType.Resource:
                    LuaFileManager.instance.SetRoot(rootPath, LuaPackageMode.Resource);
                    break;
                case RootType.LocalStream:
                    LuaFileManager.instance.SetRoot(rootPath, LuaPackageMode.LocalStream);
                    break;
            }
        }
    }
}