using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua.Components {
    [AddComponentMenu("M8/Lua/Components/FixedUpdate")]
    public class FixedUpdateComponent : BaseUpdateComponent {
        void FixedUpdate() {
            try {
                mScript.Call(mUpdate);
            }
            catch(InterpreterException ie) {
                Debug.LogError(ie.DecoratedMessage);
            }
        }
    }
}