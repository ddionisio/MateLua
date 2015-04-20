using UnityEngine;

using MoonSharp.Interpreter;

namespace M8.Lua.Modules {
    public class LuaBehaviourTable {
        private LuaBehaviour[] mBehaviours;

        public LuaBehaviour this[string index] {
            get {
                for(int i = 0; i < mBehaviours.Length; i++) {
                    if(mBehaviours[i].name == index)
                        return mBehaviours[i];
                }

                return null;
            }
        }

        public LuaBehaviour this[int index] {
            get {
                return mBehaviours[index];
            }
        }

        public LuaBehaviourTable(GameObject go) {
            mBehaviours = go.GetComponentsInChildren<LuaBehaviour>(true);
        }
    }

    public struct GameObjectModule {
        private GameObject mGo;
        private LuaBehaviourTable mBehaviours;
        private LuaBehaviour mOwner;

        public static GameObject Instantiate(GameObject go) {
            return Object.Instantiate(go) as GameObject;
        }

        public static GameObject Instantiate(GameObject go, Vector3 pos, Quaternion rot) {
            return Object.Instantiate(go, pos, rot) as GameObject;
        }
                
        public string name { get { return mGo.name; } }
        public bool activeSelf { get { return mGo.activeSelf; } }
        public bool activeInHierarchy { get { return mGo.activeInHierarchy; } }
        public int layer { get { return mGo.layer; } set { mGo.layer = value; } }
        public string layerName { get { return LayerMask.LayerToName(mGo.layer); } set { mGo.layer = LayerMask.NameToLayer(value); } }
        public string tag { get { return mGo.tag; } set { mGo.tag = value; } }
        public Transform transform { get { return mGo.transform; } }
        public LuaBehaviour owner { get { return mOwner; } }
        public LuaBehaviourTable behaviours { 
            get {
                if(mBehaviours == null)
                    mBehaviours = new LuaBehaviourTable(mGo);

                return mBehaviours;
            } 
        }

        public GameObjectModule(GameObject go, LuaBehaviour _owner = null) {
            mGo = go;
            mOwner = _owner;
            mBehaviours = null;
        }

        public void SetActive(bool a) {
            mGo.SetActive(a);
        }

        public override string ToString() {
            return mGo.name;
        }

        private static bool _isTypeRegistered = false;
        public static void Register(Table table, GameObject go, LuaBehaviour _owner = null) {
            if(!_isTypeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<GameObjectModule>();
                Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<GameObject>(itm => MoonSharp.Interpreter.UserData.Create(new GameObjectModule(itm)));
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.UserData, typeof(GameObject), itm => itm.ToObject<GameObjectModule>().mGo);

                MoonSharp.Interpreter.UserData.RegisterType<LuaBehaviourTable>();

                _isTypeRegistered = true;
            }

            table["gameObject"] = new GameObjectModule(go, _owner);
        }
    }
}