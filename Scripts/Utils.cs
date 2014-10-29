#define MATE_LUA_TRACE

using UnityEngine;

using UniLua;

namespace M8.Lua {
    public struct Utils {
        public const int nil = (int)LuaType.LUA_TNIL;

        public const string GETTER = "get";
        public const string SETTER = "set";

        public static string GetMetaName(System.Type type) {
            return type.ToString()+".meta";
        }

        public static string GetMetaName(string typeName) {
            return typeName+".meta";
        }

        public static void SetMetaTableByType(ILuaState lua, System.Type type) {
            lua.SetMetaTable(GetMetaName(type));
        }

        public static void SetMetaTableByType(ILuaState lua, string typeName) {
            lua.SetMetaTable(GetMetaName(typeName));
        }

        public static T CheckUnityObject<T>(ILuaState lua, int ind) where T : Object {
            T obj = lua.ToUserData(ind) as T;
            if(obj == null)
                lua.L_ArgError(ind, "Not a "+typeof(T));
            return obj;
        }

        public static void NewMeta(ILuaState lua, System.Type type, NameFuncPair[] metaFuncs) {
            lua.NewMetaTable(GetMetaName(type));

            lua.PushString("__index");
            lua.PushValue(-2); //meta
            lua.SetTable(-3); //meta.__index = meta

            lua.L_SetFuncs(metaFuncs, 0);
        }

        public static void NewMetaGetterSetter(ILuaState lua, System.Type type, NameFuncPair[] metaFuncs) {
            lua.NewMetaTable(GetMetaName(type));
            lua.L_SetFuncs(metaFuncs, 0);

            lua.PushString("__index");
            lua.PushString(GETTER);
            lua.GetTable(-3); //lib.getter
            lua.SetTable(-3); //meta.__index = lib.getter

            lua.PushString("__newindex");
            lua.PushString(SETTER);
            lua.GetTable(-3); //lib.setter
            lua.SetTable(-3); //meta.__newindex = lib.setter
        }

        /// <summary>
        /// Returns LuaType.LUA_TNIL if method is not found.
        /// </summary>
        public static int GetMethod(ILuaState lua, string name) {
            lua.GetField(-1, name);
            if(!lua.IsFunction(-1)) {
                lua.Pop(1);
                return nil;
            }

            return lua.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        }

        /// <summary>
        /// Call the given funcRef (from GetMethod)
        /// </summary>
        public static void CallMethod(ILuaState lua, int funcRef) {
            lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);

#if MATE_LUA_TRACE
            // insert `traceback' function
            var b = lua.GetTop();
            lua.PushCSharpFunction(Traceback);
            lua.Insert(b);

            var status = lua.PCall(0, 0, b);
#else
            var status = lua.PCall(0, 0, 0);
#endif
            if(status != ThreadStatus.LUA_OK)
                Debug.LogError(lua.ToString(-1));

#if MATE_LUA_TRACE
            // remove `traceback' function
            lua.Remove(b);
#endif
        }

        public static void CallMethod<T>(ILuaState lua, int funcRef, T objArg) {
            lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);

#if MATE_LUA_TRACE
            // insert `traceback' function
            var b = lua.GetTop();
            lua.PushCSharpFunction(Traceback);
            lua.Insert(b);
#endif

            lua.NewUserData(objArg);
            SetMetaTableByType(lua, typeof(T));

#if MATE_LUA_TRACE
            var status = lua.PCall(1, 0, b);
#else
            var status = lua.PCall(1, 0, 0);
#endif
            if(status != ThreadStatus.LUA_OK)
                Debug.LogError(lua.ToString(-1));

#if MATE_LUA_TRACE
            // remove `traceback' function
            lua.Remove(b);
#endif
        }

        public static int Traceback(ILuaState lua) {
            var msg = lua.ToString(1);
            if(msg != null) {
                lua.L_Traceback(lua, msg, 1);
            }
            // is there an error object?
            else if(!lua.IsNoneOrNil(1)) {
                // try its `tostring' metamethod
                if(!lua.L_CallMeta(1, "__tostring")) {
                    lua.PushString("(no error message)");
                }
            }
            return 1;
        }

        public static Vector3 TableToVector3(ILuaState lua, int index) {
            Vector3 ret;

            lua.L_CheckType(index, LuaType.LUA_TTABLE);

            lua.GetField(index, "x");
            lua.L_ArgCheck(!lua.IsNil(-1), index, "x is nil");
            ret.x = (float)lua.ToNumber(-1);
            lua.Pop(1);

            lua.GetField(index, "y");
            lua.L_ArgCheck(!lua.IsNil(-1), index, "y is nil");
            ret.y = (float)lua.ToNumber(-1);
            lua.Pop(1);

            lua.GetField(index, "z");
            lua.L_ArgCheck(!lua.IsNil(-1), index, "z is nil");
            ret.z = (float)lua.ToNumber(-1);
            lua.Pop(1);

            return ret;
        }
    }
}