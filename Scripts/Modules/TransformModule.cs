using UnityEngine;

using MoonSharp.Interpreter;

namespace M8.Lua.Modules {
    public struct TransformModule {
        private Transform mTrans;

        public TransformModule(Transform t) {
            mTrans = t;
        }

        public int childCount { get { return mTrans.childCount; } }
        public Vector3 eulerAngles { get { return mTrans.eulerAngles; } set { mTrans.eulerAngles = value; } }
        public Vector3 forward { get { return mTrans.forward; } set { mTrans.forward = value; } }
        public bool hasChanged { get { return mTrans.hasChanged; } set { mTrans.hasChanged = value; } }
        public Vector3 localEulerAngles { get { return mTrans.localEulerAngles; } set { mTrans.localEulerAngles = value; } }
        public Vector3 localPosition { get { return mTrans.localPosition; } set { mTrans.localPosition = value; } }
        public Quaternion localRotation { get { return mTrans.localRotation; } set { mTrans.localRotation = value; } }
        public Vector3 localScale { get { return mTrans.localScale; } set { mTrans.localScale = value; } }
        public Matrix4x4 localToWorldMatrix { get { return mTrans.localToWorldMatrix; } }
        public Vector3 lossyScale { get { return mTrans.lossyScale; } }
        public Transform parent { get { return mTrans.parent; } set { mTrans.parent = value; } }
        public Vector3 position { get { return mTrans.position; } set { mTrans.position = value; } }
        public Vector3 right { get { return mTrans.right; } set { mTrans.right = value; } }
        public Transform root { get { return mTrans.root; } }
        public Quaternion rotation { get { return mTrans.rotation; } set { mTrans.rotation = value; } }
        public Vector3 up { get { return mTrans.up; } set { mTrans.up = value; } }
        public Matrix4x4 worldToLocalMatrix { get { return mTrans.worldToLocalMatrix; } }
        public void DetachChildren() { mTrans.DetachChildren(); }
        public Transform Find(string name) { return mTrans.Find(name); }
        public Transform FindChild(string name) { return mTrans.FindChild(name); }
        public Transform GetChild(int index) { return mTrans.GetChild(index); }
        //public IEnumerator GetEnumerator();
        public int GetSiblingIndex() { return mTrans.GetSiblingIndex(); }
        public Vector3 InverseTransformDirection(Vector3 direction) { return mTrans.InverseTransformDirection(direction); }
        //public Vector3 InverseTransformDirection(float x, float y, float z);
        //
        public Vector3 InverseTransformPoint(Vector3 position) { return mTrans.InverseTransformPoint(position); }
        //public Vector3 InverseTransformPoint(float x, float y, float z);
        //
        public Vector3 InverseTransformVector(Vector3 vector) { return mTrans.InverseTransformVector(vector); }
        //public Vector3 InverseTransformVector(float x, float y, float z);
        //
        public bool IsChildOf(Transform parent) { return mTrans.IsChildOf(parent); }
        public void LookAtTransform(Transform target) { mTrans.LookAt(target); }
        public void LookAt(Vector3 worldPosition) { mTrans.LookAt(worldPosition); }
        //
        public void LookAtTransformUp(Transform target, Vector3 worldUp) { mTrans.LookAt(target, worldUp); }
        public void LookAtUp(Vector3 worldPosition, Vector3 worldUp) { mTrans.LookAt(worldPosition, worldUp); }
        public void Rotate(Vector3 eulerAngles, Space space = Space.Self) { mTrans.Rotate(eulerAngles, space); }
        public void RotateAround(Vector3 axis, float angle, Space space = Space.Self) { mTrans.Rotate(axis, angle, space); }
        public void SetAsFirstSibling() { mTrans.SetAsFirstSibling(); }
        public void SetAsLastSibling() { mTrans.SetAsLastSibling(); }
        public void SetParent(Transform parent, bool worldPositionStays = true) { mTrans.SetParent(parent, worldPositionStays); }
        public void SetSiblingIndex(int index) { mTrans.SetSiblingIndex(index); }
        public Vector3 TransformDirection(Vector3 direction) { return mTrans.TransformDirection(direction); }
        public Vector3 TransformPoint(Vector3 position) { return mTrans.TransformPoint(position); }
        public Vector3 TransformVector(Vector3 vector) { return mTrans.TransformVector(vector); }
        public void Translate(Vector3 translation, Space space = Space.Self) { mTrans.Translate(translation, space); }
        public void TranslateRelativeTo(Vector3 translation, Transform relativeTo) { mTrans.Translate(translation, relativeTo); }
        
        private static bool _isTypeRegistered = false;
        public static void Register(Table table, Transform t) {
            if(!_isTypeRegistered) {
                MoonSharp.Interpreter.UserData.RegisterType<TransformModule>();
                Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Transform>(itm => MoonSharp.Interpreter.UserData.Create(new TransformModule(itm)));
                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.UserData, typeof(Transform), itm => itm.ToObject<TransformModule>().mTrans);

                Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.String, typeof(Space), itm => System.Enum.Parse(typeof(Space), itm.String));

                _isTypeRegistered = true;
            }

            table["transform"] = new TransformModule(t);
        }
    }
}