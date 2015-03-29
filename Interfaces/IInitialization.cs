
using MoonSharp.Interpreter;

namespace M8.Lua {
    public interface IInitialization {
        /// <summary>
        /// Called before loading and executing the given script.
        /// </summary>
        void PreLoad(Script s);

        /// <summary>
        /// Called after given script has been loaded and executed.
        /// </summary>
        void PostLoad(Script s);
    }
}