using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua.Components {
    [AddComponentMenu("M8/Lua/Components/Update")]
    public class UpdateComponent : BaseUpdateComponent {
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