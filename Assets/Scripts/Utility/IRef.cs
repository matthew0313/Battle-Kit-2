using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class IRef<T> : ISerializationCallbackReceiver where T : class
{
    UnityEngine.Object prevTarget;
    public UnityEngine.Object target;
    public T I => target as T;
    public void OnBeforeSerialize() => OnValidate();
    public void OnAfterDeserialize() { }
    void OnValidate()
    {
        if (!(target is T))
        {
            if (target is GameObject go)
            {
                target = prevTarget;
                foreach (Component c in go.GetComponents<Component>())
                {
                    if (c is T)
                    {
                        target = c;
                        break;
                    }
                }
            }
            else target = prevTarget;
        }
        prevTarget = target;
    }
}