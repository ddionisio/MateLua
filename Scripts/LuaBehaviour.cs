#define MATE_LUA_TRACE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UniLua;

namespace M8.Lua {
    [AddComponentMenu("M8/Lua/Behaviour")]
    public class LuaBehaviour : MonoBehaviour, ILuaInitializer {
        private const string luaMethodAwake = "awake";
        private const string luaMethodStart = "start";
        private const string luaMethodOnEnable = "onEnable";
        private const string luaMethodOnDisable = "onDisable";
        private const string luaMethodOnDestroy = "onDestroy";

        private const string luaMethodUpdate = "update";
        private const string luaMethodLateUpdate = "lateUpdate";
        private const string luaMethodFixedUpdate = "fixedUpdate";

        private const string luaMethodTriggerEnter = "onTriggerEnter";
        private const string luaMethodTriggerStay = "onTriggerStay";
        private const string luaMethodTriggerExit = "onTriggerExit";

        private const string luaMethodCollisionEnter = "onCollisionEnter";
        private const string luaMethodCollisionStay = "onCollisionStay";
        private const string luaMethodCollisionExit = "onCollisionExit";

        //mate
        private const string luaMethodOnSpawned = "onSpawned";
        private const string luaMethodOnDespawned = "onDespawned";
                
        public string scriptPath; //path to lua file
        public TextAsset scriptText; //if code path is empty, use this for loading

        private ILuaState mLua;

        //unity stuff
        private int mAwakeInd;
        private int mLuaMethodStart;
        private int mLuaMethodOnEnable;
        private int mLuaMethodOnDisable;
        private int mLuaMethodOnDestroy;

        //mate stuff
        private int mLuaMethodOnSpawned;
        private int mLuaMethodOnDespawned;
                
        public ILuaState lua { get { return mLua; } }

        //Mate Calls

        void OnSpawned() {
            if(mLuaMethodOnSpawned != Utils.nil)
                Utils.CallMethod(mLua, mLuaMethodOnSpawned);
        }

        void OnDespawned() {
            if(mLuaMethodOnDespawned != Utils.nil)
                Utils.CallMethod(mLua, mLuaMethodOnDespawned);
        }

        //Unity Calls
                
        void OnDestroy() {
            if(mLuaMethodOnDestroy != Utils.nil)
                Utils.CallMethod(mLua, mLuaMethodOnDestroy);
        }

        void OnEnable() {
            if(mLuaMethodOnEnable != Utils.nil)
                Utils.CallMethod(mLua, mLuaMethodOnEnable);
        }

        void OnDisable() {
            StopAllCoroutines();

            if(mLuaMethodOnDisable != Utils.nil)
                Utils.CallMethod(mLua, mLuaMethodOnDisable);
        }

        void ILuaInitializer.LuaRequire(ILuaState lua) {
            //common unity libs
            lua.L_RequireF(Library.UnityObject.LIB_NAME, Library.UnityObject.OpenLib, false);
            lua.L_RequireF(Library.UnityGameObject.LIB_NAME, Library.UnityGameObject.OpenLib, false);
            lua.L_RequireF(Library.UnityComponent.LIB_NAME, Library.UnityComponent.OpenLib, false);
            lua.L_RequireF(Library.UnityBehaviour.LIB_NAME, Library.UnityBehaviour.OpenLib, false);
            lua.L_RequireF(Library.UnityTransform.LIB_NAME, Library.UnityTransform.OpenLib, false);
            lua.L_RequireF(Library.UnityCollider.LIB_NAME, Library.UnityCollider.OpenLib, false);
            lua.L_RequireF(Library.UnityBounds.LIB_NAME, Library.UnityBounds.OpenLib, false);
            lua.L_RequireF(Library.UnityRigidbody.LIB_NAME, Library.UnityRigidbody.OpenLib, false);
            lua.L_RequireF(Library.UnityCollision.LIB_NAME, Library.UnityCollision.OpenLib, false);

            //meta-only objects
            Library.UnityContactPoint.DefineMeta(lua);
        }

        void ILuaInitializer.LuaPreExecute(ILuaState lua) {
            //add some variables
            lua.NewUserData(gameObject);
            lua.SetMetaTable(Library.UnityGameObject.META_NAME);
            lua.SetGlobal("gameObject");

            lua.NewUserData(transform);
            lua.SetMetaTable(Library.UnityTransform.META_NAME);
            lua.SetGlobal("transform");

            //add some functions
            lua.PushCSharpFunction(LuaInvoke);
            lua.SetGlobal("invoke");

            lua.PushCSharpFunction(LuaInvokeRepeat);
            lua.SetGlobal("invokeRepeat");

            lua.PushCSharpFunction(LuaCancelInvoke);
            lua.SetGlobal("cancelInvoke");

            lua.PushCSharpFunction(LuaCancelAllInvoke);
            lua.SetGlobal("cancelAllInvoke");
        }

        void ILuaInitializer.LuaPostExecute(ILuaState lua) {
            //grab callbacks from table
            mAwakeInd = Utils.GetMethod(lua, luaMethodAwake);

            mLuaMethodStart = Utils.GetMethod(lua, luaMethodStart);
            mLuaMethodOnEnable = Utils.GetMethod(lua, luaMethodOnEnable);
            mLuaMethodOnDisable = Utils.GetMethod(lua, luaMethodOnDisable);
            mLuaMethodOnDestroy = Utils.GetMethod(lua, luaMethodOnDestroy);

            mLuaMethodOnSpawned = Utils.GetMethod(lua, luaMethodOnSpawned);
            mLuaMethodOnDespawned = Utils.GetMethod(lua, luaMethodOnDespawned);

            int updateInd = Utils.GetMethod(lua, luaMethodUpdate);
            if(updateInd != Utils.nil) {
                M8.Auxiliary.AuxUpdate aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxUpdate>(gameObject);
                aux.callback += delegate() { Utils.CallMethod(lua, updateInd); };
            }

            int fixedUpdateInd = Utils.GetMethod(lua, luaMethodFixedUpdate);
            if(fixedUpdateInd != Utils.nil) {
                M8.Auxiliary.AuxFixedUpdate aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxFixedUpdate>(gameObject);
                aux.callback += delegate() { Utils.CallMethod(lua, fixedUpdateInd); };
            }

            int lateUpdateInd = Utils.GetMethod(lua, luaMethodLateUpdate);
            if(lateUpdateInd != Utils.nil) {
                M8.Auxiliary.AuxLateUpdate aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxLateUpdate>(gameObject);
                aux.callback += delegate() { Utils.CallMethod(lua, lateUpdateInd); };
            }

            int triggerEnterInd = Utils.GetMethod(lua, luaMethodTriggerEnter);
            int triggerStayInd = Utils.GetMethod(lua, luaMethodTriggerStay);
            int triggerExitInd = Utils.GetMethod(lua, luaMethodTriggerExit);
            if(triggerEnterInd != Utils.nil || triggerStayInd != Utils.nil || triggerExitInd != Utils.nil) {
                M8.Auxiliary.AuxTrigger aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxTrigger>(gameObject);
                if(triggerEnterInd != Utils.nil) aux.enterCallback += delegate(Collider c) { Utils.CallMethod<Collider>(lua, triggerEnterInd, c, Library.UnityCollider.META_NAME); };
                if(triggerStayInd != Utils.nil) aux.stayCallback += delegate(Collider c) { Utils.CallMethod<Collider>(lua, triggerStayInd, c, Library.UnityCollider.META_NAME); };
                if(triggerExitInd != Utils.nil) aux.exitCallback += delegate(Collider c) { Utils.CallMethod<Collider>(lua, triggerExitInd, c, Library.UnityCollider.META_NAME); };
            }

            int collEnterInd = Utils.GetMethod(lua, luaMethodCollisionEnter);
            int collStayInd = Utils.GetMethod(lua, luaMethodCollisionStay);
            int collExitInd = Utils.GetMethod(lua, luaMethodCollisionExit);
            if(collEnterInd != Utils.nil || collStayInd != Utils.nil || collExitInd != Utils.nil) {
                M8.Auxiliary.AuxCollision aux = M8.Util.GetOrAddComponent<M8.Auxiliary.AuxCollision>(gameObject);
                if(collEnterInd != Utils.nil) aux.enterCallback += delegate(Collision c) { Utils.CallMethod<Collision>(lua, collEnterInd, c, Library.UnityCollision.META_NAME); };
                if(collStayInd != Utils.nil) aux.stayCallback += delegate(Collision c) { Utils.CallMethod<Collision>(lua, collStayInd, c, Library.UnityCollision.META_NAME); };
                if(collExitInd != Utils.nil) aux.exitCallback += delegate(Collision c) { Utils.CallMethod<Collision>(lua, collExitInd, c, Library.UnityCollision.META_NAME); };
            }
        }
                
        void Awake() {
            //grab lua initializers
            Component[] comps = GetComponents<Component>();
            List<ILuaInitializer> inits = new List<ILuaInitializer>(comps.Length);
            for(int i = 0; i < comps.Length; i++) {
                ILuaInitializer ilinit = comps[i] as ILuaInitializer;
                if(ilinit != null)
                    inits.Add(ilinit);
            }

            //init lua
            mLua = LuaAPI.NewState();
            mLua.L_OpenLibs();
                        
            //setup requires
            for(int i = 0; i < inits.Count; i++)
                inits[i].LuaRequire(mLua);

            mLua.Pop(mLua.GetTop());
            
            //setup global fields and functions
            for(int i = 0; i < inits.Count; i++)
                inits[i].LuaPreExecute(mLua);
            
            //execute
            ThreadStatus status = string.IsNullOrEmpty(scriptPath) ? mLua.L_DoString(scriptText.text) : mLua.L_DoFile(scriptPath);
            if(status != ThreadStatus.LUA_OK) {
                throw new Exception(mLua.ToString(-1));
            }

            if(!mLua.IsTable(-1)) {
                throw new Exception("Lua script's return value is not a table.");
            }

            //additional setups
            for(int i = 0; i < inits.Count; i++)
                inits[i].LuaPostExecute(mLua);
                        
            /*mLua.CreateTable(0, 2);
            mLua.PushLightUserData(gameObject);
            mLua.SetField(-2, "__go");
            mLua.PushCSharpFunction(goname);
            mLua.SetField(-2, "name");*/
                                                                        
            //awake
            if(mAwakeInd != Utils.nil)
                Utils.CallMethod(mLua, mAwakeInd);
        }

        /*static int goname(ILuaState l) {
            l.SetTop(1);
            l.L_CheckType(1, LuaType.LUA_TTABLE);
                        
            l.GetField(1, "__go");
            GameObject go = l.ToUserData(-1) as GameObject;

            l.Pop(1);

            l.PushString(go.name);

            return 1;
        }*/

        // Use this for initialization
        void Start() {
            if(mLuaMethodStart != Utils.nil)
                Utils.CallMethod(mLua, mLuaMethodStart);
        }

        //Internal
        
        private int GetFuncRef(ILuaState lua, int ind) {
            int funcRef = 0;

            if(lua.IsFunction(ind)) {
                lua.PushValue(ind);
                funcRef = lua.L_Ref(LuaDef.LUA_REGISTRYINDEX);
            }
            else if(lua.IsString(ind)) {
                string f = lua.ToString(ind);
                lua.GetGlobal(f);
                if(lua.IsFunction(-1))
                    funcRef = lua.L_Ref(LuaDef.LUA_REGISTRYINDEX);
                else {
                    lua.Pop(1);
                    lua.L_ArgError(ind, "Function not found: "+f);
                }
            }
            else
                lua.L_ArgError(ind, "Argument is not a function or string.");

            return funcRef;
        }
                
        private int LuaInvoke(ILuaState lua) {

            int funcRef = GetFuncRef(lua, 1);
            if(funcRef == 0) return 0;
               
            float time = (float)lua.L_CheckNumber(2);

            //TODO: n-arguments to pass
            //Debug.Log("narg: "+lua.GetTop());
            IEnumerator e = DoInvoke(lua, funcRef, time);

            StartCoroutine(e);

            lua.PushLightUserData(e); //keep this if you want to cancel invoke
            return 1;
        }

        private int LuaInvokeRepeat(ILuaState lua) {

            int funcRef = GetFuncRef(lua, 1);
            if(funcRef == 0) return 0;

            float time = (float)lua.L_CheckNumber(2);
            float repeat = (float)lua.L_CheckNumber(3);

            //TODO: n-arguments to pass
            //Debug.Log("narg: "+lua.GetTop());
            IEnumerator e = DoInvokeRepeat(lua, funcRef, time, repeat);

            StartCoroutine(e);

            lua.PushLightUserData(e); //keep this if you want to cancel invoke
            return 1;
        }

        private int LuaCancelInvoke(ILuaState lua) {
            IEnumerator e = lua.ToUserData(1) as IEnumerator;
            if(e != null)
                StopCoroutine(e);
            return 0;
        }

        private int LuaCancelAllInvoke(ILuaState lua) {
            StopAllCoroutines();
            return 0;
        }

        private IEnumerator DoInvoke(ILuaState lua, int funcRef, float time) {
            if(time > 0)
                yield return new WaitForSeconds(time);
            else
                yield return null;

            lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
            ThreadStatus status = lua.PCall(0, 0, 0);
            if(status != ThreadStatus.LUA_OK)
                lua.L_Error("Error running function: "+lua.L_ToString(-1));
        }

        private IEnumerator DoInvokeRepeat(ILuaState lua, int funcRef, float time, float repeatRate) {
            if(time > 0)
                yield return new WaitForSeconds(time);
            else
                yield return null;

            WaitForSeconds wait = new WaitForSeconds(repeatRate);
            while(true) {
                lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
                ThreadStatus status = lua.PCall(0, 0, 0);
                if(status != ThreadStatus.LUA_OK) {
                    lua.L_Error("Error running function: "+lua.L_ToString(-1));
                    break;
                }
                yield return wait;
            }
        }
    }
}