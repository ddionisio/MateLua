using UnityEngine;

using System.Text;
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
        public void AddPackage(string basePath, LuaPackageMode mode, object data, ILuaPackageReader api) {
            mPackages.Add(new LuaPackage() { basePath=basePath, mode=mode, data=data, api=api });
        }

        public void RemovePackage(string basePath) {
            for(int i = 0; i < mPackages.Count; i++) {
                LuaPackage package = mPackages[i];
                if(package.basePath == basePath) {
                    mPackages.RemoveAt(i);
                    break;
                }
            }
        }

        public void ClearPackages() {
            mPackages.Clear();
        }

        internal ILoadStreamer Open(string filename) {
            //try packages first, starting from end
            for(int i = mPackages.Count - 1; i >= 0; i--) {
                ILoadStreamer loader = mPackages[i].Open(filename, mStrBuff);
                if(loader != null)
                    return loader;
            }

            //try root
            return mDefault.Open(filename, mStrBuff);
        }

        internal bool Readable(string filename) {
            //try packages first, starting from end
            for(int i = mPackages.Count - 1; i >= 0; i--) {
                if(mPackages[i].Readable(filename, mStrBuff))
                    return true;
            }

            //try root
            return mDefault.Readable(filename, mStrBuff);
        }

        private static LuaFileManager mInstance = null;

        private List<LuaPackage> mPackages = new List<LuaPackage>();

        private StringBuilder mStrBuff = new StringBuilder(512);

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
        public ILoadStreamer Open(string filepath, StringBuilder sb) {
            int dotInd, fpCount;
            string fullpath;

            switch(mode) {
                case LuaPackageMode.Resource:
                    //truncate extension
                    dotInd = filepath.LastIndexOf('.');
                                        
                    sb.Length = 0;

                    if(!string.IsNullOrEmpty(basePath)) {
                        sb.Append(basePath);
                        sb.Append('/');
                    }

                    //copy filepath, while replacing \ to /
                    fpCount = dotInd == -1 ? filepath.Length : dotInd;
                    for(int i = 0; i < fpCount; i++) {
                        char c = filepath[i];
                        sb.Append(c == '\\' ? '/' : c);
                    }

                    TextAsset ta = Resources.Load<TextAsset>(sb.ToString());
                    return ta ? new ByteLoadStreamer(ta.bytes) : null;

                case LuaPackageMode.LocalStream:
                    sb.Length = 0;
                    sb.Append(Application.streamingAssetsPath);
                    sb.Append('/');
                    if(!string.IsNullOrEmpty(basePath)) {
                        sb.Append(basePath);
                        sb.Append('/');
                    }
                    sb.Append(filepath);

                    fullpath = sb.ToString();
                    return File.Exists(fullpath) ? new FileLoadStreamer(fullpath) : null;

                case LuaPackageMode.Bundle:
                    //TODO: async?
                    AssetBundle ab = data as AssetBundle;

                    //truncate extension
                    dotInd = filepath.LastIndexOf('.');

                    sb.Length = 0;

                    if(!string.IsNullOrEmpty(basePath)) {
                        sb.Append(basePath);
                        sb.Append('/');
                    }

                    //copy filepath, while replacing \ to /
                    fpCount = dotInd == -1 ? filepath.Length : dotInd;
                    for(int i = 0; i < fpCount; i++) {
                        char c = filepath[i];
                        sb.Append(c == '\\' ? '/' : c);
                    }

                    fullpath = sb.ToString();
                    return ab.Contains(fullpath) ? new ByteLoadStreamer((ab.Load(fullpath) as TextAsset).bytes) : null;

                case LuaPackageMode.Custom:
                    return api.Open(data, basePath, filepath);
            }
            return null;
        }

        /// <summary>
        /// Check if filepath exists. filepath is relative to this package's root
        /// </summary>
        public bool Readable(string filepath, StringBuilder sb) {
            int dotInd, fpCount;

            switch(mode) {
                case LuaPackageMode.Resource:
                    //truncate extension
                    dotInd = filepath.LastIndexOf('.');

                    sb.Length = 0;

                    if(!string.IsNullOrEmpty(basePath)) {
                        sb.Append(basePath);
                        sb.Append('/');
                    }

                    //copy filepath, while replacing \ to /
                    fpCount = dotInd == -1 ? filepath.Length : dotInd;
                    for(int i = 0; i < fpCount; i++) {
                        char c = filepath[i];
                        sb.Append(c == '\\' ? '/' : c);
                    }

                    //WTF, why is there no Exist or Contains function?
                    //TODO: cache?
                    return Resources.Load(sb.ToString()) != null;

                case LuaPackageMode.LocalStream:
                    sb.Length = 0;
                    sb.Append(Application.streamingAssetsPath);
                    sb.Append('/');
                    if(!string.IsNullOrEmpty(basePath)) {
                        sb.Append(basePath);
                        sb.Append('/');
                    }
                    sb.Append(filepath);
                    return File.Exists(sb.ToString());

                case LuaPackageMode.Bundle:
                    AssetBundle ab = data as AssetBundle;

                    //truncate extension
                    dotInd = filepath.LastIndexOf('.');

                    sb.Length = 0;

                    if(!string.IsNullOrEmpty(basePath)) {
                        sb.Append(basePath);
                        sb.Append('/');
                    }

                    //copy filepath, while replacing \ to /
                    fpCount = dotInd == -1 ? filepath.Length : dotInd;
                    for(int i = 0; i < fpCount; i++) {
                        char c = filepath[i];
                        sb.Append(c == '\\' ? '/' : c);
                    }

                    return ab.Contains(sb.ToString());

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