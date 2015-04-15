using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua.Components {
    [AddComponentMenu("M8/Lua/Components/Update")]
    public class UpdateComponent : MonoBehaviour, IInitialization {
        private Script mScript;
        private DynValue mUpdate;

        void IInitialization.PreLoad(Script s) {

        }

        void IInitialization.PostLoad(Script s) {
            mScript = s;
            mUpdate = s.Globals.Get(Const.luaFuncUpdate);
            if(mUpdate.IsNilOrNan()) {
                enabled = false;
                Debug.LogError(string.Format("{0} not found.", Const.luaFuncUpdate));
            }
        }

        void Update() {
            try {
                mScript.Call(mUpdate);
            }
            catch(InterpreterException ie) {
                Debug.LogError(ie.DecoratedMessage);
            }
        }
    }
}