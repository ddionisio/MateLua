using UnityEngine;
using System.Collections;

namespace M8.Lua {
    [AddComponentMenu("M8/Lua/Behaviour")]
    public class LuaBehaviour : MonoBehaviour {
        public TextAsset codeRef; //optional as reference
        
        void Awake() {
            Object t = Resources.Load("Lua/lib/ffi", typeof(TextAsset));
            Debug.Log(t);
        }

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }
    }
}