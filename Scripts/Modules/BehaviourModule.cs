using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua.Modules {
    public struct BehaviourModule {
        private LuaBehaviour mBehaviour;

        public BehaviourModule(LuaBehaviour b) {
            mBehaviour = b;
        }

        public bool enabled { get { return mBehaviour.enabled; } set { mBehaviour.enabled = value; } }
        public bool isActiveAndEnabled { get { return mBehaviour.isActiveAndEnabled; } }
        public GameObject gameObject { get { return mBehaviour.gameObject; } }
        public DynValue globals { get { return DynValue.NewTable(mBehaviour.script.Globals); } }

        public UnityEngine.Coroutine Invoke(DynValue func, DynValue param) {
            if(func.Type == DataType.String) {
                //try to grab from global
                func = mBehaviour.script.Globals.Get(func);
            }

            return mBehaviour.StartCoroutine(InvokeRoutine(mBehaviour.script, mBehaviour, func, param));
        }

        public void CancelInvoke(UnityEngine.Coroutine coro) {
            mBehaviour.StopCoroutine(coro);
        }

        public void CancelAllInvoke() {
            mBehaviour.StopAllCoroutines();
        }
     
        private static bool _isTypeRegistered = false;
        public static void Register(Table table, LuaBehaviour b) {
            if(!_isTypeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<BehaviourModule>();

                Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<LuaBehaviour>(itm => MoonSharp.Interpreter.UserData.Create(new BehaviourModule(itm)));
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.UserData, typeof(LuaBehaviour), itm => itm.ToObject<BehaviourModule>().mBehaviour);

                _isTypeRegistered = true;
            }

            table["behaviour"] = new BehaviourModule(b);
        }

        public static IEnumerator InvokeRoutine(Script script, MonoBehaviour behaviour, DynValue func, params DynValue[] param) {
            DynValue coFunc;
            switch(func.Type) {
                case DataType.Thread:
                    coFunc = func;
                    break;
                case DataType.Function:
                case DataType.ClrFunction:
                    coFunc = script.CreateCoroutine(func);
                    break;
                default:
                    Debug.LogError(string.Format("'{0}' ({1}) is not a valid function/coroutine.", func, func.Type));
                    yield break;
            }

            var routine = coFunc.Coroutine;

            while(routine.State == CoroutineState.NotStarted || routine.State == CoroutineState.Suspended) {
                DynValue val = null;

                try {
                    val = routine.Resume(param);
                }
                catch(InterpreterException ie) {
                    Debug.LogError(ie.DecoratedMessage);
                    yield break;
                }

                switch(val.Type) {
                    case DataType.Thread:
                        yield return behaviour.StartCoroutine(InvokeRoutine(script, behaviour, val));
                        break;
                    case DataType.Tuple:
                        var tuple = val.Tuple;
                        if(tuple[0].Type == DataType.Thread) {
                            if(tuple.Length > 1) {
                                var subParam = new DynValue[tuple.Length - 1];
                                System.Array.Copy(tuple, 1, subParam, 0, tuple.Length - 1);
                                yield return behaviour.StartCoroutine(InvokeRoutine(script, behaviour, tuple[0], subParam));
                            }
                            else
                                yield return behaviour.StartCoroutine(InvokeRoutine(script, behaviour, tuple[0]));
                        }
                        else
                            yield return null;
                        break;
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
    }
}