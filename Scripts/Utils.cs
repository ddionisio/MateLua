using UnityEngine;

using UniLua;

namespace M8.Lua {
    public struct Utils {
        public static T CheckObject<T>(ILuaState lua, int ind) where T : Object {
            T obj = lua.ToUserData(ind) as T;
            if(obj == null)
                lua.L_ArgError(ind, "Not a "+typeof(T));
            return obj;
        }
    }
}