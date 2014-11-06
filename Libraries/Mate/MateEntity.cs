using UnityEngine;
using System.Collections;

using UniLua;

namespace M8.Lua.Library {
    public static class MateEntity {
        public const string LIB_NAME = "Mate.Entity";

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

                    new NameFuncPair("activate", Activate),
                    new NameFuncPair("release", Release),

                    new NameFuncPair("addSetStateCall", AddSetStateCall),
                    new NameFuncPair("removeSetStateCall", RemoveSetStateCall),
                    new NameFuncPair("addSpawnCall", AddSpawnCall),
                    new NameFuncPair("removeSpawnCall", RemoveSpawnCall),
                    new NameFuncPair("addReleaseCall", AddReleaseCall),
                    new NameFuncPair("removeReleaseCall", RemoveReleaseCall),
                };
            if(l_funcs == null)
                l_funcs = new NameFuncPair[] {
                };

            Utils.NewMetaGetterSetter(lua, typeof(EntityBase), m_funcs);
            lua.L_NewLib(l_funcs);

            return 1;
        }

        public static void Push(ILuaState lua, EntityBase ent) {
            if(ent) {
                lua.NewUserData(ent);
                Utils.SetMetaTableByType(lua, typeof(EntityBase));
            }
            else //null given,
                lua.PushNil();
        }

        private static int Get(ILuaState lua) {
            EntityBase ent = Utils.CheckUnityObject<EntityBase>(lua, 1);
            string field = lua.L_CheckString(2);
            switch(field) {
                case "spawnType":
                    lua.PushString(ent.spawnType);
                    break;
                case "spawnGroup":
                    lua.PushString(ent.spawnGroup);
                    break;
                case "state":
                    lua.PushInteger(ent.state);
                    break;
                case "prevState":
                    lua.PushInteger(ent.prevState);
                    break;
                case "isReleased":
                    lua.PushBoolean(ent.isReleased);
                    break;
                case "serializer":
                    lua.NewUserData(ent.serializer);
                    Utils.SetMetaTableByType(lua, typeof(SceneSerializer));
                    break;
                default:
                    if(!UnityBehaviour.PushField(lua, ent, field))
                        if(!lua.L_GetMetaField(1, field))
                            lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 1;
        }

        private static int Set(ILuaState lua) {
            EntityBase ent = Utils.CheckUnityObject<EntityBase>(lua, 1);
            string field = lua.L_CheckString(2);
            switch(field) {
                case "state":
                    ent.state = lua.L_CheckInteger(3);
                    break;
                default:
                    if(!UnityBehaviour.SetField(lua, ent, field))
                        lua.L_Error("Unknown field: {0}", field);
                    break;
            }
            return 0;
        }

        private static int Activate(ILuaState lua) {
            EntityBase ent = Utils.CheckUnityObject<EntityBase>(lua, 1);
            ent.Activate();
            return 0;
        }

        private static int Release(ILuaState lua) {
            EntityBase ent = Utils.CheckUnityObject<EntityBase>(lua, 1);
            ent.Release();
            return 0;
        }

        private static int AddSetStateCall(ILuaState lua) {
            EntityBase ent = Utils.CheckUnityObject<EntityBase>(lua, 1);

            int funcRef = Utils.GetFuncRef(lua, 2);
            if(funcRef > 0) {
                EntityBase.OnGenericCall call = delegate(EntityBase e) {
                    lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);

                    lua.NewUserData(e);
                    Utils.SetMetaTableByType(lua, e.GetType());

                    ThreadStatus status = lua.PCall(1, 0, 0);
                    if(status != ThreadStatus.LUA_OK)
                        lua.L_Error("Error running function: "+lua.L_ToString(-1));
                };

                ent.setStateCallback += call;

                lua.PushLightUserData(call);
            }
            else
                lua.PushNil();
            return 1;
        }

        private static int RemoveSetStateCall(ILuaState lua) {
            EntityBase ent = Utils.CheckUnityObject<EntityBase>(lua, 1);
            EntityBase.OnGenericCall call = lua.ToUserData(2) as EntityBase.OnGenericCall;
            if(call != null)
                ent.setStateCallback -= call;
            else
                lua.L_ArgError(2, "Not a delegate: EntityBase.OnGenericCall");
            return 0;
        }

        private static int AddSpawnCall(ILuaState lua) {
            EntityBase ent = Utils.CheckUnityObject<EntityBase>(lua, 1);

            int funcRef = Utils.GetFuncRef(lua, 2);
            if(funcRef > 0) {
                EntityBase.OnGenericCall call = delegate(EntityBase e) {
                    lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);

                    lua.NewUserData(e);
                    Utils.SetMetaTableByType(lua, e.GetType());

                    ThreadStatus status = lua.PCall(1, 0, 0);
                    if(status != ThreadStatus.LUA_OK)
                        lua.L_Error("Error running function: "+lua.L_ToString(-1));
                };

                ent.spawnCallback += call;

                lua.PushLightUserData(call);
            }
            else
                lua.PushNil();
            return 1;
        }

        private static int RemoveSpawnCall(ILuaState lua) {
            EntityBase ent = Utils.CheckUnityObject<EntityBase>(lua, 1);
            EntityBase.OnGenericCall call = lua.ToUserData(2) as EntityBase.OnGenericCall;
            if(call != null)
                ent.spawnCallback -= call;
            else
                lua.L_ArgError(2, "Not a delegate: EntityBase.OnGenericCall");
            return 0;
        }

        private static int AddReleaseCall(ILuaState lua) {
            EntityBase ent = Utils.CheckUnityObject<EntityBase>(lua, 1);

            int funcRef = Utils.GetFuncRef(lua, 2);
            if(funcRef > 0) {
                EntityBase.OnGenericCall call = delegate(EntityBase e) {
                    lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);

                    lua.NewUserData(e);
                    Utils.SetMetaTableByType(lua, e.GetType());

                    ThreadStatus status = lua.PCall(1, 0, 0);
                    if(status != ThreadStatus.LUA_OK)
                        lua.L_Error("Error running function: "+lua.L_ToString(-1));
                };

                ent.releaseCallback += call;

                lua.PushLightUserData(call);
            }
            else
                lua.PushNil();
            return 1;
        }

        private static int RemoveReleaseCall(ILuaState lua) {
            EntityBase ent = Utils.CheckUnityObject<EntityBase>(lua, 1);
            EntityBase.OnGenericCall call = lua.ToUserData(2) as EntityBase.OnGenericCall;
            if(call != null)
                ent.releaseCallback -= call;
            else
                lua.L_ArgError(2, "Not a delegate: EntityBase.OnGenericCall");
            return 0;
        }
    }
}