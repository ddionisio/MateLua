using UnityEngine;
using System.Collections;

namespace M8.Lua {
    public class LoaderFromResourceManager : LoaderBase {
        public override bool ScriptFileExists(string file) {
            throw new System.NotImplementedException();
        }

        public override object LoadFile(string file, MoonSharp.Interpreter.Table globalContext) {
            throw new System.NotImplementedException();
        }
    }
}