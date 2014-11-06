using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class MateMusic {
        public const string LIB_NAME = "Mate.Music";

        private static NameFuncPair[] m_funcs = null;
        private static NameFuncPair[] l_funcs = null;

        public static int OpenLib(ILuaState lua) {
            if(m_funcs == null)
                m_funcs = new NameFuncPair[] {
                    new NameFuncPair(Utils.GETTER, Get),
                    new NameFuncPair(Utils.SETTER, Set),
                };
            if(l_funcs == null)
                l_funcs = new NameFuncPair[] {
                    new NameFuncPair("Exists", Exists),
                    new NameFuncPair("Play", Play),
                    new NameFuncPair("Stop", Stop),
                };

            Utils.NewMetaGetterSetter(lua, typeof(MusicManager), m_funcs);
            lua.L_NewLib(l_funcs);

            Utils.SetMetaTableByType(lua, typeof(MusicManager));

            return 1;
        }

        private static int Get(ILuaState lua) {
            string field = lua.L_CheckString(2);
            switch(field) {
                case "isPlaying":
                    lua.PushBoolean(MusicManager.instance.isPlaying);
                    break;
                default:
                    if(!lua.L_GetMetaField(1, field))
                        lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 1;
        }

        private static int Set(ILuaState lua) {
            string field = lua.L_CheckString(2);
            switch(field) {
                default:
                    lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 0;
        }

        private static int Exists(ILuaState lua) {
            string name = lua.L_CheckString(1);
            lua.PushBoolean(MusicManager.instance.Exists(name));
            return 1;
        }

        private static int Play(ILuaState lua) {
            string name = lua.L_CheckString(1);
            bool immediate = lua.GetTop() >= 2 ? lua.ToBoolean(2) : false;
            MusicManager.instance.Play(name, immediate);
            return 0;
        }

        private static int Stop(ILuaState lua) {
            bool fade = lua.GetTop() >= 1 ? lua.ToBoolean(1) : true;
            MusicManager.instance.Stop(fade);
            return 0;
        }
    }

    public static class MateSoundGlobal {
        public const string LIB_NAME = "Mate.SoundGlobal";

        private static NameFuncPair[] l_funcs = null;

        public static int OpenLib(ILuaState lua) {
            if(l_funcs == null)
                l_funcs = new NameFuncPair[] {
                    new NameFuncPair("Play", Play),
                    new NameFuncPair("Stop", Stop),
                };

            lua.L_NewLib(l_funcs);

            return 1;
        }

        private static int Play(ILuaState lua) {
            string name = lua.L_CheckString(1);
            GameObject go = SoundPlayerGlobal.instance.Play(name);
            lua.PushLightUserData(go);
            return 1;
        }

        private static int Stop(ILuaState lua) {
            GameObject go = Utils.CheckUnityObject<GameObject>(lua, 1);
            if(go)
                SoundPlayerGlobal.instance.Stop(go);
            return 0;
        }
    }

    public static class MateSound {
        public const string LIB_NAME = "Mate.Sound";

        private static NameFuncPair[] m_funcs = null;
        private static NameFuncPair[] l_funcs = null;

        public static int OpenLib(ILuaState lua) {
            if(m_funcs == null)
                m_funcs = new NameFuncPair[] {
                    new NameFuncPair(Utils.GETTER, Get),
                    new NameFuncPair(Utils.SETTER, Set),

                    new NameFuncPair("compareTag", UnityComponent.CompareTag),
                    new NameFuncPair("getComponent", UnityComponent.GetComponent),
                    new NameFuncPair("getComponentInChildren", UnityComponent.GetComponentInChildren),
                    new NameFuncPair("getComponentInParent", UnityComponent.GetComponentInParent),

                    new NameFuncPair("sendMessage", UnityComponent.SendMessage),
                    new NameFuncPair("sendMessageUpwards", UnityComponent.SendMessageUpwards),
                    new NameFuncPair("broadcastMessage", UnityComponent.BroadcastMessage),

                    new NameFuncPair("play", Play),
                    new NameFuncPair("stop", Stop),
                };
            if(l_funcs == null)
                l_funcs = new NameFuncPair[] {
                };

            Utils.NewMetaGetterSetter(lua, typeof(SoundPlayer), m_funcs);
            lua.L_NewLib(l_funcs);

            return 1;
        }

        public static void Push(ILuaState lua, SoundPlayer snd) {
            if(snd) {
                lua.NewUserData(snd);
                Utils.SetMetaTableByType(lua, typeof(SoundPlayer));
            }
            else //null given,
                lua.PushNil();
        }

        private static int Get(ILuaState lua) {
            SoundPlayer snd = Utils.CheckUnityObject<SoundPlayer>(lua, 1);
            string field = lua.L_CheckString(2);
            switch(field) {
                case "isPlaying":
                    lua.PushBoolean(snd.isPlaying);
                    break;
                case "defaultVolume":
                    lua.PushNumber(snd.defaultVolume);
                    break;
                default:
                    if(!UnityBehaviour.PushField(lua, snd, field))
                        if(!lua.L_GetMetaField(1, field))
                            lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 1;
        }

        private static int Set(ILuaState lua) {
            SoundPlayer snd = Utils.CheckUnityObject<SoundPlayer>(lua, 1);
            string field = lua.L_CheckString(2);
            switch(field) {
                case "defaultVolume":
                    snd.defaultVolume = (float)lua.L_CheckNumber(3);
                    break;
                default:
                    if(!UnityBehaviour.SetField(lua, snd, field))
                        lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 0;
        }

        private static int Play(ILuaState lua) {
            SoundPlayer snd = Utils.CheckUnityObject<SoundPlayer>(lua, 1);
            snd.Play();
            return 0;
        }

        private static int Stop(ILuaState lua) {
            SoundPlayer snd = Utils.CheckUnityObject<SoundPlayer>(lua, 1);
            snd.Stop();
            return 0;
        }
    }
}