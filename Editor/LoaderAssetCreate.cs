using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8.Lua {
    static class LoaderAssetCreate {
        [MenuItem("Assets/Create/Lua/Loaders/FromResources")]
        static void CreateLoaderFromResources() {
            M8.EditorExt.Utility.CreateAssetToCurrentSelectionFolder<LoaderFromResources>();
        }
    }
}