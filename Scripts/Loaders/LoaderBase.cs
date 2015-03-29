using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace M8.Lua {
    public abstract class LoaderBase : ScriptableObject, IScriptLoader {
        [Tooltip("Prepend to path to determine the absolute path.  Ensure there is no leading slash.")]
        public string rootDir = "Lua";

        [Tooltip("Gets or sets the modules paths used by the \"require\" function. If null, the default paths are used (using environment variables etc.).")]
        public string[] modulePaths;

        /// <summary>
        /// Checks if a script file exists. 
        /// </summary>
        /// <param name="name">The script filename.</param>
        /// <returns></returns>
        public abstract bool ScriptFileExists(string file);

        /// <summary>
        /// Opens a file for reading the script code.
        /// It can return either a string, a byte[] or a Stream.
        /// If a byte[] is returned, the content is assumed to be a serialized (dumped) bytecode. If it's a string, it's
        /// assumed to be either a script or the output of a string.dump call. If a Stream, autodetection takes place.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="globalContext">The global context.</param>
        /// <returns>
        /// A string, a byte[] or a Stream.
        /// </returns>
        public abstract object LoadFile(string file, Table globalContext);

        /// <summary>
        /// Resolves the name of a module on a set of paths.
        /// </summary>
        /// <param name="modname">The modname.</param>
        /// <param name="paths">The paths.</param>
        /// <returns></returns>
        protected virtual string ResolveModuleName(string modname, string[] paths) {
            modname = modname.Replace('.', '/');

            foreach(string path in paths) {
                string file = path.Replace("?", modname);

                if(ScriptFileExists(file))
                    return file;
            }

            return null;
        }

        /// <summary>
        /// Resolves the name of a module to a filename (which will later be passed to OpenScriptFile).
        /// The resolution happens first on paths included in the LUA_PATH global variable, and -
        /// if the variable does not exist - by consulting the
        /// ScriptOptions.ModulesPaths array. Override to provide a different behaviour.
        /// </summary>
        /// <param name="modname">The modname.</param>
        /// <param name="globalContext">The global context.</param>
        /// <returns></returns>
        public virtual string ResolveModuleName(string modname, Table globalContext) {
            DynValue s = globalContext.RawGet("LUA_PATH");

            if(s != null && s.Type == DataType.String)
                return ResolveModuleName(modname, ScriptLoaderBase.UnpackStringPaths(s.String));

            return ResolveModuleName(modname, modulePaths);
        }

        /// <summary>
        /// Resolves a filename [applying paths, etc.]
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="globalContext">The global context.</param>
        /// <returns></returns>
        public string ResolveFileName(string filename, Table globalContext) {
            return filename;
        }
    }
}