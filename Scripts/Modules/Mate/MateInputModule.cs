using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MoonSharp.Interpreter;

namespace M8.Lua.Modules {
    public struct MateInputInfo {
        public int action;
        public float axis;
        public int index;

        private InputManager.State _state;

        public string actionName { get { return InputManager.instance.GetActionName(action); } }
        public bool isReleased { get { return _state == InputManager.State.Released; } }
        public bool isPressed { get { return _state == InputManager.State.Pressed; } }

        public MateInputInfo(InputManager.Info inf) {
            action = inf.action;
            axis = inf.axis;
            index = inf.index;
            _state = inf.state;
        }
    }

    /// <summary>
    /// Allow to set which player to grab input from.  Use index as player, e.g.: Mate.Input[1] = player 1
    /// Note: index start is 1, for lua consistency
    /// </summary>
    public class MateInputModule {
        public static int actionCount { get { return InputManager.instance.actionCount; } }

        public static int GetActionIndex(string actionName) {
            return InputManager.instance.GetActionIndex(actionName);
        }

        public static string GetActionName(int action) {
            return InputManager.instance.GetActionName(action);
        }
        //

        public static float GetAxis(int player, int action) {
            return InputManager.instance.GetAxis(player, action);
        }

        public static float GetAxis(int player, string action) {
            return InputManager.instance.GetAxis(player, action);
        }

        public static bool IsPressed(int player, int action) {
            return InputManager.instance.IsPressed(player, action);
        }

        public static bool IsPressed(int player, string action) {
            return InputManager.instance.IsPressed(player, action);
        }

        public static bool IsReleased(int player, int action) {
            return InputManager.instance.IsReleased(player, action);
        }

        public static bool IsReleased(int player, string action) {
            return InputManager.instance.IsReleased(player, action);
        }

        public static bool IsDown(int player, int action) {
            return InputManager.instance.IsDown(player, action);
        }

        public static bool IsDown(int player, string action) {
            return InputManager.instance.IsDown(player, action);
        }

        public static int GetIndex(int player, int action) {
            return InputManager.instance.GetIndex(player, action);
        }

        public static int GetIndex(int player, string action) {
            return InputManager.instance.GetIndex(player, action);
        }

        public static void ClearButtonCall(int action) {
            InputManager.instance.ClearButtonCall(action);
        }

        public static void ClearButtonCall(string action) {
            InputManager.instance.ClearButtonCall(action);
        }

        public static void ClearAllButtonCalls() {
            InputManager.instance.ClearAllButtonCalls();
        }

        private Script mScript;
        private Dictionary<DynValue, InputManager.OnButton> mBinds;

        public MateInputModule(Script script) {
            mScript = script;
            mBinds = new Dictionary<DynValue, InputManager.OnButton>();
        }

        /// <summary>
        /// ensure the first 3 params are: player, action, function
        /// </summary>
        public void AddButtonCall(CallbackArguments args) {
            int player = System.Convert.ToInt32(args[0].Number);

            int action;

            DynValue actionVal = args[1];
            if(actionVal.Type == DataType.String) {
                action = GetActionIndex(actionVal.String);
            }
            else
                action = System.Convert.ToInt32(actionVal.CastToNumber());
            

            DynValue luaFunc = args[2];

            //setup parameters, first element will be MateInputInfo
            DynValue[] parms = new DynValue[args.Count-3+1];
            System.Array.Copy(args.GetArray(3), 0, parms, 1, parms.Length - 1);

            if(luaFunc.Type == DataType.String)
                luaFunc = mScript.Globals.Get(luaFunc);

            InputManager.OnButton func;

            switch(luaFunc.Type) {
                case DataType.Function:
                case DataType.ClrFunction:
                    func = delegate(InputManager.Info inf) {
                        try {
                            //call lua function with parameters, first being the info
                            parms[0] = DynValue.FromObject(mScript, new MateInputInfo(inf));
                            mScript.Call(luaFunc, parms);
                        }
                        catch(InterpreterException ie) {
                            Debug.LogError(ie.DecoratedMessage);
                        }
                    };
                    break;

                default:
                    Debug.LogError(string.Format("{0} ({1}) is not a function or routine.", luaFunc, luaFunc.Type));
                    return;
            }

            if(mBinds.ContainsKey(luaFunc)) {
                InputManager.instance.RemoveButtonCall(mBinds[luaFunc]);
                mBinds[luaFunc] = func;
            }
            else
                mBinds.Add(luaFunc, func);

            InputManager.instance.AddButtonCall(player, action, func);
        }

        public void RemoveButtonCall(DynValue luaFunc) {
            InputManager.OnButton func;
            if(mBinds.TryGetValue(luaFunc, out func)) {
                InputManager.instance.RemoveButtonCall(func);
                mBinds.Remove(luaFunc);
            }
        }

        public void ClearButtonCalls() {
            var input = InputManager.instance;
            foreach(var pair in mBinds)
                input.RemoveButtonCall(pair.Value);

            mBinds.Clear();
        }

        private static bool _isTypeRegistered = false;
        public static void Register(Table table) {
            if(!_isTypeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<MateInputInfo>();
                MoonSharp.Interpreter.UserData.RegisterType<MateInputModule>();
                _isTypeRegistered = true;
            }

            table["Input"] = new MateInputModule(table.OwnerScript);
        }

    }
}