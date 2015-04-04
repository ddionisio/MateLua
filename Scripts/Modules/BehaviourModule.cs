using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua.Modules {
    public struct BehaviourModule {
        private Script mScript;
        private MonoBehaviour mBehaviour;

        public BehaviourModule(Script script, MonoBehaviour b) {
            mScript = script;
            mBehaviour = b;
        }

        public UnityEngine.Coroutine Invoke(DynValue func, float delay, DynValue param) {
            return mBehaviour.StartCoroutine(DoInvoke(func, delay, param));
        }

        public UnityEngine.Coroutine InvokeRepeat(DynValue func, float startDelay, float repeatDelay, DynValue param) {
            return mBehaviour.StartCoroutine(DoInvokeRepeat(func, startDelay, repeatDelay, param));
        }

        public UnityEngine.Coroutine InvokeRoutine(DynValue func, DynValue param) {
            return mBehaviour.StartCoroutine(DoInvokeRoutine(func, param));
        }

        public void CancelInvoke(UnityEngine.Coroutine coro, DynValue param) {
            mBehaviour.StopCoroutine(coro);
        }

        IEnumerator DoInvoke(DynValue func, float delay, DynValue param) {
            if(delay > 0f)
                yield return new WaitForSeconds(delay);

            mScript.Call(func, param);
        }

        IEnumerator DoInvokeRepeat(DynValue func, float startDelay, float repeatDelay, DynValue param) {
            if(startDelay > 0f)
                yield return new WaitForSeconds(startDelay);

            WaitForSeconds wait = new WaitForSeconds(repeatDelay);

            while(true) {
                mScript.Call(func, param);

                yield return wait;
            }
        }

        IEnumerator DoInvokeRoutine(DynValue func, DynValue param) {
            DynValue coFunc;
            switch(func.Type) {
                case DataType.Thread:
                    coFunc = func;
                    break;
                default:
                    coFunc = mScript.CreateCoroutine(func);
                    break;
            }

            var routine = coFunc.Coroutine;

            while(routine.State == CoroutineState.NotStarted || routine.State == CoroutineState.Suspended) {
                DynValue val = routine.Resume(param);

                switch(val.Type) {
                    case DataType.UserData:
                        YieldInstruction y = val.ToObject<YieldInstruction>();
                        if(y != null) {
                            yield return y;
                            continue;
                        }

                        UnityEngine.Coroutine coro = val.ToObject<UnityEngine.Coroutine>();
                        if(coro != null) {
                            yield return coro;
                            continue;
                        }

                        Debug.LogWarning("Cannot yield: "+val.UserData.Descriptor.Name);
                        yield return null;
                        break;
                    default:
                        yield return null;
                        break;
                }
            }
        }

        private static bool _isTypeRegistered = false;
        public static void Register(Table table, MonoBehaviour b) {
            if(!_isTypeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<BehaviourModule>();

                _isTypeRegistered = true;
            }

            table["behaviour"] = new BehaviourModule(table.OwnerScript, b);
        }
    }
}