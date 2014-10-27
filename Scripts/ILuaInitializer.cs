
using UniLua;

/// <summary>
/// Use for setting up the lua environment within LuaBehaviour, put this in your component along with luabehaviour for it to be used.
/// </summary>
public interface ILuaInitializer {
    /// <summary>
    /// Called for setting up any requires and defining metatables
    /// </summary>
    void LuaRequire(ILuaState lua);

    /// <summary>
    /// Called for setting up the environment before executing the lua file, add fields and functions here.
    /// </summary>
    /// <param name="lua"></param>
    void LuaPreExecute(ILuaState lua);

    /// <summary>
    /// Called once the lua environment has executed, the top stack is the returned table from lua, use this to setup callbacks (e.g. OnCollision, etc.)
    /// </summary>
    void LuaPostExecute(ILuaState lua);
}
