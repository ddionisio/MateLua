using UnityEngine;

using System;
using System.IO;
using System.Collections.Generic;

namespace UniLua {
    public interface ILoadStreamer {
        int ReadByte();
        void Dispose();
    }

    public interface ILuaPackageReader {
        ILoadStreamer LuaPackageReader_Open(object packageData, string basePath, string filepath);
        bool LuaPackageReader_Readable(object packageData, string basePath, string filepath);
    }

    public struct LuaPackage {
        public enum Mode {
            Resource,
            LocalStream,
            Bundle,
            Custom
        }

        public Mode mode;
        public string basePath; //prepend
        public object data; //AssetBundle for bundle, anything for custom
        public ILuaPackageReader api; //for custom

        /// <summary>
        /// Open given file. filepath is relative to this package's basePath
        /// </summary>
        public ILoadStreamer Open(string filepath) {
            int dotInd;

            switch(mode) {
                case Mode.Resource:
                    //truncate extension
                    dotInd = filepath.LastIndexOf('.');
                    if(dotInd != -1) filepath = filepath.Substring(0, dotInd);

                    if(!string.IsNullOrEmpty(basePath)) {
                        using(StringWriter sb = new StringWriter()) {
                            sb.Write(basePath);
                            sb.Write('/');
                            sb.Write(filepath);
                            return new ResourceLoadStreamer(sb.ToString());
                        }
                    }
                    else
                        return new ResourceLoadStreamer(filepath);

                case Mode.LocalStream:
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

                case Mode.Bundle:
                    //TODO: async?
                    AssetBundle ab = data as AssetBundle;

                    //truncate extension
                    dotInd = filepath.LastIndexOf('.');
                    if(dotInd != -1) filepath = filepath.Substring(0, dotInd);

                    if(!string.IsNullOrEmpty(basePath)) {
                        using(StringWriter sb = new StringWriter()) {
                            sb.Write(basePath);
                            sb.Write('/');
                            sb.Write(filepath);
                                                        
                            return new ByteLoadStreamer((ab.Load(sb.ToString()) as TextAsset).bytes);
                        }
                    }
                    else
                        return new ByteLoadStreamer((ab.Load(filepath) as TextAsset).bytes);

                case Mode.Custom:
                    return api.LuaPackageReader_Open(data, basePath, filepath);
            }
            return null;
        }

        /// <summary>
        /// Check if filepath exists. filepath is relative to this package's root
        /// </summary>
        public bool Readable(string filepath) {
            int dotInd;

            switch(mode) {
                case Mode.Resource:
                    //truncate extension
                    dotInd = filepath.LastIndexOf('.');
                    if(dotInd != -1) filepath = filepath.Substring(0, dotInd);

                    if(!string.IsNullOrEmpty(basePath)) {
                        using(StringWriter sb = new StringWriter()) {
                            sb.Write(basePath);
                            sb.Write('/');
                            sb.Write(filepath);

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

                case Mode.LocalStream:
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

                case Mode.Bundle:
                    AssetBundle ab = data as AssetBundle;

                    //truncate extension
                    dotInd = filepath.LastIndexOf('.');
                    if(dotInd != -1) filepath = filepath.Substring(0, dotInd);

                    if(!string.IsNullOrEmpty(basePath)) {
                        using(StringWriter sb = new StringWriter()) {
                            sb.Write(basePath);
                            sb.Write('/');
                            sb.Write(filepath);

                            return ab.Contains(sb.ToString());
                        }
                    }
                    else
                        return ab.Contains(filepath);

                case Mode.Custom:
                    return api.LuaPackageReader_Readable(data, basePath, filepath);
            }

            return false;
        }
    }

    public struct LuaFileManager {
        private static Dictionary<string, LuaPackage> mPackages = new Dictionary<string, LuaPackage>();

        //default is Resources, with root dir: Lua
        private static LuaPackage mDefault = new LuaPackage() { mode=LuaPackage.Mode.Resource, basePath="Lua" };

        /// <summary>
        /// Note: If mode is Bundle, ensure that the data is an AssetBundle
        /// </summary>
        public static void SetRoot(string basePath, LuaPackage.Mode mode, object data, ILuaPackageReader api) {
            mDefault.mode = mode;
            mDefault.basePath = basePath;
            mDefault.data = data;
            mDefault.api = api;
        }

        /// <summary>
        /// Note: If mode is Bundle, ensure that the data is an AssetBundle
        /// </summary>
        public static void AddPackage(string name, string basePath, LuaPackage.Mode mode, object data, ILuaPackageReader api) {
            LuaPackage package = new LuaPackage() { basePath=basePath, mode=mode, data=data, api=api };
            if(mPackages.ContainsKey(name))
                mPackages[name] = package;
            else
                mPackages.Add(name, package);
        }

        public static void RemovePackage(string name) {
            mPackages.Remove(name);
        }

        internal static ILoadStreamer Open(string filename) {
            string packageFilePath;
            return GrabPackage(filename, out packageFilePath).Open(packageFilePath);
        }

        internal static bool Readable(string filename) {
            string packageFilePath;
            return GrabPackage(filename, out packageFilePath).Readable(packageFilePath);
        }

        private static readonly char[] dirDelim = new char[] { '\\', '/' };

        private static LuaPackage GrabPackage(string filename, out string packageFilePath) {
            string packageName;

            int splitInd = filename.IndexOfAny(dirDelim);
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
            int ret = mBytes[mPos];
            mPos++;
            return ret;
        }

        public void Dispose() {
            Resources.UnloadAsset(mAsset);
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