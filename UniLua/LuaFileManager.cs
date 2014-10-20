using UnityEngine;

using System;
using System.IO;
using System.Collections.Generic;

namespace UniLua {
    public enum LuaPackageMode {
        Resource, //All lua files will be read via Resources
        LocalStream, //All lua files will be read from Application.streamingAssetsPath
        Bundle, //All lua files will be read from AssetBundle
        Custom //
    }

    public interface ILoadStreamer {
        /// <summary>
        /// Read a byte, return -1 if end of stream
        /// </summary>
        int ReadByte();

        void Dispose();
    }

    public interface ILuaPackageReader {
        ILoadStreamer Open(object packageData, string basePath, string filepath);
        bool Readable(object packageData, string basePath, string filepath);
    }

    public class LuaFileManager {
        public const string defaultBasePath = "Lua";

        public static LuaFileManager instance { 
            get {
                if(mInstance == null)
                    mInstance = new LuaFileManager();
                return mInstance;
            } 
        }
                
        /// <summary>
        /// Note: If mode is Bundle, ensure that the data is an AssetBundle
        /// </summary>
        public void SetRoot(string basePath, LuaPackageMode mode, object data = null, ILuaPackageReader api = null) {
            mDefault.mode = mode;
            mDefault.basePath = basePath;
            mDefault.data = data;
            mDefault.api = api;
        }

        /// <summary>
        /// Note: If mode is Bundle, ensure that the data is an AssetBundle
        /// </summary>
        public void AddPackage(string name, string basePath, LuaPackageMode mode, object data, ILuaPackageReader api) {
            LuaPackage package = new LuaPackage() { basePath=basePath, mode=mode, data=data, api=api };
            if(mPackages.ContainsKey(name))
                mPackages[name] = package;
            else
                mPackages.Add(name, package);
        }

        public void RemovePackage(string name) {
            mPackages.Remove(name);
        }

        internal ILoadStreamer Open(string filename) {
            string packageFilePath;
            return GrabPackage(filename, out packageFilePath).Open(packageFilePath);
        }

        internal bool Readable(string filename) {
            string packageFilePath;
            return GrabPackage(filename, out packageFilePath).Readable(packageFilePath);
        }

        private readonly char[] mDirDelim = new char[] { '\\', '/' };

        private LuaPackage GrabPackage(string filename, out string packageFilePath) {
            string packageName;

            int splitInd = filename.IndexOfAny(mDirDelim);
            if(splitInd != -1) {
                packageName = filename.Substring(0, splitInd);
                packageFilePath = filename.Substring(splitInd+1);
            }
            else { //no folder path?
                packageFilePath = filename;
                return mDefault;
            }

            //see if packageName matches, otherwise assume it is in default package
            LuaPackage package;
            if(!mPackages.TryGetValue(packageName, out package)) {
                packageFilePath = filename;
                return mDefault;
            }

            return package;
        }

        private static LuaFileManager mInstance = null;

        private Dictionary<string, LuaPackage> mPackages = new Dictionary<string, LuaPackage>();

        //default is Resources, with root dir: Lua
        private LuaPackage mDefault = new LuaPackage() { mode=LuaPackageMode.Resource, basePath=defaultBasePath };
    }

    internal struct LuaPackage {
        public LuaPackageMode mode;
        public string basePath; //prepend
        public object data; //AssetBundle for bundle, anything for custom
        public ILuaPackageReader api; //for custom

        /// <summary>
        /// Open given file. filepath is relative to this package's basePath
        /// </summary>
        public ILoadStreamer Open(string filepath) {
            int dotInd;

            switch(mode) {
                case LuaPackageMode.Resource:
                    //truncate extension
                    dotInd = filepath.LastIndexOf('.');

                    if(!string.IsNullOrEmpty(basePath)) {
                        using(StringWriter sb = new StringWriter()) {
                            sb.Write(basePath);
                            sb.Write('/');

                            //copy filepath, while replacing \ to /
                            int fpCount = dotInd == -1 ? filepath.Length : dotInd;
                            for(int i = 0; i < fpCount; i++) {
                                char c = filepath[i];
                                sb.Write(c == '\\' ? '/' : c);
                            }

                            return new ResourceLoadStreamer(sb.ToString());
                        }
                    }
                    else
                        return new ResourceLoadStreamer(filepath);

                case LuaPackageMode.LocalStream:
                    using(StringWriter sb = new StringWriter()) {
                        sb.Write(Application.streamingAssetsPath);
                        sb.Write('/');
                        if(!string.IsNullOrEmpty(basePath)) {
                            sb.Write(basePath);
                            sb.Write('/');
                        }
                        sb.Write(filepath);
                        return new FileLoadStreamer(sb.ToString());
                    }

                case LuaPackageMode.Bundle:
                    //TODO: async?
                    AssetBundle ab = data as AssetBundle;

                    //truncate extension
                    dotInd = filepath.LastIndexOf('.');

                    if(!string.IsNullOrEmpty(basePath)) {
                        using(StringWriter sb = new StringWriter()) {
                            sb.Write(basePath);
                            sb.Write('/');

                            //copy filepath, while replacing \ to /
                            int fpCount = dotInd == -1 ? filepath.Length : dotInd;
                            for(int i = 0; i < fpCount; i++) {
                                char c = filepath[i];
                                sb.Write(c == '\\' ? '/' : c);
                            }
                                                        
                            return new ByteLoadStreamer((ab.Load(sb.ToString()) as TextAsset).bytes);
                        }
                    }
                    else
                        return new ByteLoadStreamer((ab.Load(filepath) as TextAsset).bytes);

                case LuaPackageMode.Custom:
                    return api.Open(data, basePath, filepath);
            }
            return null;
        }

        /// <summary>
        /// Check if filepath exists. filepath is relative to this package's root
        /// </summary>
        public bool Readable(string filepath) {
            int dotInd;

            switch(mode) {
                case LuaPackageMode.Resource:
                    //truncate extension
                    dotInd = filepath.LastIndexOf('.');

                    if(!string.IsNullOrEmpty(basePath)) {
                        using(StringWriter sb = new StringWriter()) {
                            sb.Write(basePath);
                            sb.Write('/');

                            //copy filepath, while replacing \ to /
                            int fpCount = dotInd == -1 ? filepath.Length : dotInd;
                            for(int i = 0; i < fpCount; i++) {
                                char c = filepath[i];
                                sb.Write(c == '\\' ? '/' : c);
                            }

                            //WTF, why is there no Exist or Contains function?
                            //TODO: cache?
                            UnityEngine.Object obj = Resources.Load(sb.ToString());
                            if(obj) {
                                Resources.UnloadAsset(obj);
                                return true;
                            }
                            else
                                return false;
                        }
                    }
                    else {
                        UnityEngine.Object obj = Resources.Load(filepath);
                        if(obj) {
                            Resources.UnloadAsset(obj);
                            return true;
                        }
                        else
                            return false;
                    }   

                case LuaPackageMode.LocalStream:
                    using(StringWriter sb = new StringWriter()) {
                        sb.Write(Application.streamingAssetsPath);
                        sb.Write('/');
                        if(!string.IsNullOrEmpty(basePath)) {
                            sb.Write(basePath);
                            sb.Write('/');
                        }
                        sb.Write(filepath);
                        return File.Exists(sb.ToString());
                    }

                case LuaPackageMode.Bundle:
                    AssetBundle ab = data as AssetBundle;

                    //truncate extension
                    dotInd = filepath.LastIndexOf('.');

                    if(!string.IsNullOrEmpty(basePath)) {
                        using(StringWriter sb = new StringWriter()) {
                            sb.Write(basePath);
                            sb.Write('/');

                            //copy filepath, while replacing \ to /
                            int fpCount = dotInd == -1 ? filepath.Length : dotInd;
                            for(int i = 0; i < fpCount; i++) {
                                char c = filepath[i];
                                sb.Write(c == '\\' ? '/' : c);
                            }

                            return ab.Contains(sb.ToString());
                        }
                    }
                    else
                        return ab.Contains(filepath);

                case LuaPackageMode.Custom:
                    return api.Readable(data, basePath, filepath);
            }

            return false;
        }
    }

    internal class FileLoadStreamer : ILoadStreamer {
        public int ReadByte() {
            return mStream.ReadByte();
        }

        public void Dispose() {
            mStream.Dispose();
        }

        public FileLoadStreamer(string path) {
            mStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private FileStream mStream;
    }

    internal class ResourceLoadStreamer : ILoadStreamer {
        public int ReadByte() {
            if(mPos == mBytes.Length) return -1;

            int ret = mBytes[mPos];
            mPos++;
            return ret;
        }

        public void Dispose() {
            //TODO: might be better to just unload elsewhere
            //Resources.UnloadAsset(mAsset);
        }

        public ResourceLoadStreamer(string path) {
            //TODO: load async?
            mAsset = Resources.Load<TextAsset>(path);
            mBytes = mAsset.bytes;
        }

        private TextAsset mAsset;
        private byte[] mBytes;
        private int mPos = 0;
    }

    internal class ByteLoadStreamer : ILoadStreamer {
        public int ReadByte() {
            if(mPos == mBytes.Length) return -1;

            int ret = mBytes[mPos];
            mPos++;
            return ret;
        }

        public void Dispose() {
        }

        public ByteLoadStreamer(byte[] bytes) {
            mBytes = bytes;
        }

        private byte[] mBytes;
        private int mPos = 0;
    }
}