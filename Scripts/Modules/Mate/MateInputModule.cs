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
        public struct BindKey {
            public DynValue func;
            public int action;
            public int player;
        }

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
        private Dictionary<BindKey, InputManager.OnButton> mBinds;

        public MateInputModule(Script script) {
            mScript = script;
            mBinds = new Dictionary<BindKey, InputManager.OnButton>();
        }

        /// <summary>
        /// ensure the first 3 params are: player, action, function
        /// </summary>
        public void AddButtonCall(int player, DynValue action, DynValue luaFunc) {
            int actionInd;
            if(action.Type == DataType.String)
                actionInd = GetActionIndex(action.String);
            else if(action.Type == DataType.Number)
                actionInd = System.Convert.ToInt32(action.Number);
            else
                throw new ScriptRuntimeException(string.Format("Invalid action: {0} ({1}) is not a string or number.", action, action.Type));

            if(luaFunc.Type == DataType.String)
                luaFunc = mScript.Globals.Get(luaFunc);

            InputManager.OnButton func;

            switch(luaFunc.Type) {
                case DataType.Function:
                case DataType.ClrFunction:
                    func = delegate(InputManager.Info inf) {
                        try {
                            //call lua function with parameters, first being the info
                            DynValue luaInfo = DynValue.FromObject(mScript, new MateInputInfo(inf));
                            mScript.Call(luaFunc, luaInfo);
                        }
                        catch(InterpreterException ie) {
                            Debug.LogError(ie.DecoratedMessage);
                        }
                    };
                    break;

                default:
                    throw new ScriptRuntimeException(string.Format("{0} ({1}) is not a function or routine.", luaFunc, luaFunc.Type));
            }

            BindKey key = new BindKey() { func=luaFunc, player=player, action=actionInd };

            if(!mBinds.ContainsKey(key)) {
                mBinds.Add(key, func);
                InputManager.instance.AddButtonCall(player, actionInd, func);
            }
        }

        public void RemoveButtonCall(int player, DynValue action, DynValue luaFunc) {
            int actionInd;
            if(action.Type == DataType.String)
                actionInd = GetActionIndex(action.String);
            else if(action.Type == DataType.Number)
                actionInd = System.Convert.ToInt32(action.Number);
            else
                throw new ScriptRuntimeException(string.Format("Invalid action: {0} ({1}) is not a string or number.", action, action.Type));

            if(luaFunc.Type == DataType.String)
                luaFunc = mScript.Globals.Get(luaFunc);

            BindKey key = new BindKey() { func=luaFunc, player=player, action=actionInd };

            InputManager.OnButton func;
            if(mBinds.TryGetValue(key, out func)) {
                InputManager.instance.RemoveButtonCall(player, actionInd, func);
                mBinds.Remove(key);
            }
        }

        public void ClearButtonCalls() {
            var input = InputManager.instance;
            foreach(var pair in mBinds)
                input.RemoveButtonCall(pair.Key.player, pair.Key.action, pair.Value);

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