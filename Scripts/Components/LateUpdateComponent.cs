using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua.Components {
    [AddComponentMenu("M8/Lua/Components/LateUpdate")]
    public class LateUpdateComponent : BaseUpdateComponent {
        void LateUpdate() {
            try {
                mScript.Call(mUpdate);
            }
            catch(InterpreterException ie) {
                Debug.LogError(ie.DecoratedMessage);
            }
        }
    }
}