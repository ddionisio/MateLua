using UnityEngine;
using System.Collections;


using MoonSharp.Interpreter;

namespace M8.Lua.Components {
    [AddComponentMenu("")]
    public abstract class BaseUpdateComponent : MonoBehaviour, IInitialization {
        protected Script mScript;
        protected DynValue mUpdate;

        void IInitialization.PreLoad(Script s) {
        }

        void IInitialization.PostLoad(Script s) {
            mScript = s;

            mUpdate = mScript.Globals.Get(Const.luaFuncUpdate);
            if(mUpdate.IsNilOrNan()) {
                enabled = false;
                Debug.LogError(string.Format("{0} not found.", Const.luaFuncUpdate));
            }
            else
                enabled = true;
        }

        void OnEnable() {
            //avoid spam
            if(mUpdate == null || mUpdate.IsNil())
                enabled = false;
        }
    }
}