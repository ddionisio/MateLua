using UnityEngine;

using MoonSharp.Interpreter;

namespace M8.Lua.Modules {
    public struct GameObjectModule {
        private GameObject mGo;
        private LuaBehaviour[] mBehaviours;
                
        public string name { get { return mGo.name; } }
        public bool activeSelf { get { return mGo.activeSelf; } }
        public bool activeInHierarchy { get { return mGo.activeInHierarchy; } }
        public int layer { get { return mGo.layer; } set { mGo.layer = value; } }
        public string layerName { get { return LayerMask.LayerToName(mGo.layer); } set { mGo.layer = LayerMask.NameToLayer(value); } }
        public string tag { get { return mGo.tag; } set { mGo.tag = value; } }
        public Transform transform { get { return mGo.transform; } }
        public LuaBehaviour behaviour { 
            get {
                var lbs = behaviours;
                return lbs.Length > 0 ? lbs[0] : null;
            }
        }
        public LuaBehaviour[] behaviours { 
            get { 
                if(mBehaviours == null)
                    mBehaviours = mGo.GetComponents<LuaBehaviour>();
                return mBehaviours;
            } 
        }

        public GameObjectModule(GameObject go) {
            mGo = go;
            mBehaviours = null;
        }

        public void SetActive(bool a) {
            mGo.SetActive(a);
        }

        public override string ToString() {
            return mGo.name;
        }

        private static bool _isTypeRegistered = false;
        public static void Register(Table table, GameObject go) {
            if(!_isTypeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<GameObjectModule>();
                Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<GameObject>(itm => MoonSharp.Interpreter.UserData.Create(new GameObjectModule(itm)));
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.UserData, typeof(GameObject), itm => itm.ToObject<GameObjectModule>().mGo);
                _isTypeRegistered = true;
            }

            table["gameObject"] = new GameObjectModule(go);
        }
    }
}