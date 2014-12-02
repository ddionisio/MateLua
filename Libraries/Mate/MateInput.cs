using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class MateInput {
        public const string LIB_NAME = "Mate.Input";

        private static NameFuncPair[] l_funcs = null;

        public static int OpenLib(ILuaState lua) {
            if(l_funcs == null)
                l_funcs = new NameFuncPair[] {
                    new NameFuncPair("GetAxis", GetAxis),
                    new NameFuncPair("IsPressed", IsPressed),
                    new NameFuncPair("IsReleased", IsReleased),
                    new NameFuncPair("IsDown", IsDown),
                    new NameFuncPair("GetIndex", GetIndex),
                    new NameFuncPair("AddButtonCall", AddButtonCall),
                    new NameFuncPair("RemoveButtonCall", RemoveButtonCall),
                    new NameFuncPair("ClearButtonCall", ClearButtonCall),
                    new NameFuncPair("ClearAllButtonCalls", ClearAllButtonCalls),
                };

            lua.L_NewLib(l_funcs);

            //constants
            lua.PushInteger((int)InputManager.State.None);
            lua.SetField(-2, "stateNone");

            lua.PushInteger((int)InputManager.State.Pressed);
            lua.SetField(-2, "statePressed");

            lua.PushInteger((int)InputManager.State.Released);
            lua.SetField(-2, "stateReleased");

            return 1;
        }
        
        private static int GetAxis(ILuaState lua) {
            int player = lua.L_CheckInteger(1);
            string action = lua.L_CheckString(2);
            float axis = InputManager.instance.GetAxis(player, action);
            lua.PushNumber(axis);
            return 1;
        }

        private static int IsPressed(ILuaState lua) {
            int player = lua.L_CheckInteger(1);
            string action = lua.L_CheckString(2);
            lua.PushBoolean(InputManager.instance.IsPressed(player, action));
            return 1;
        }

        private static int IsReleased(ILuaState lua) {
            int player = lua.L_CheckInteger(1);
            string action = lua.L_CheckString(2);
            lua.PushBoolean(InputManager.instance.IsReleased(player, action));
            return 1;
        }

        private static int IsDown(ILuaState lua) {
            int player = lua.L_CheckInteger(1);
            string action = lua.L_CheckString(2);
            lua.PushBoolean(InputManager.instance.IsDown(player, action));
            return 1;
        }

        private static int GetIndex(ILuaState lua) {
            int player = lua.L_CheckInteger(1);
            string action = lua.L_CheckString(2);
            int ind = InputManager.instance.GetIndex(player, action);
            lua.PushInteger(ind);
            return 1;
        }

        /// <summary>
        /// Be sure to call RemoveButtonCall with the returned delegate
        /// Parameters: state, axis, index
        /// Use the variables: state[None, Pressed, Released] to check the state.
        /// </summary>
        private static int AddButtonCall(ILuaState lua) {
            int player = lua.L_CheckInteger(1);
            string action = lua.L_CheckString(2);
            int funcRef = Utils.GetFuncRef(lua, 3);
            if(funcRef > 0) {
                InputManager.OnButton call = delegate(InputManager.Info data) {
                    lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);

                    lua.PushInteger((int)data.state);
                    lua.PushNumber(data.axis);
                    lua.PushInteger(data.index);

                    ThreadStatus status = lua.PCall(3, 0, 0);
                    if(status != ThreadStatus.LUA_OK)
                        lua.L_Error("Error running function: "+lua.L_ToString(-1));
                };

                InputManager.instance.AddButtonCall(player, action, call);

                lua.PushLightUserData(call);
            }
            else
                lua.PushNil();

            return 1;
        }

        private static int RemoveButtonCall(ILuaState lua) {
            InputManager input = InputManager.instance;
            if(input) {
                InputManager.OnButton call = lua.ToUserData(1) as InputManager.OnButton;
                if(call != null)
                    input.RemoveButtonCall(call);
                else
                    lua.L_ArgError(1, "Not a delegate: InputManager.OnButton");
            }
            return 0;
        }

        private static int ClearButtonCall(ILuaState lua) {
            string action = lua.L_CheckString(1);
            InputManager.instance.ClearButtonCall(action);
            return 0;
        }

        private static int ClearAllButtonCalls(ILuaState lua) {
            InputManager.instance.ClearAllButtonCalls();
            return 0;
        }
    }
}