
namespace M8.Lua {
    /// <summary>
    /// Used for containing a value to be passed between lua and C#, mostly for structs
    /// </summary>
    public interface IContainer {
        object data { get; }
    }
}