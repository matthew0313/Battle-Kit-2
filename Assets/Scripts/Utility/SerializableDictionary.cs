using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    List<TKey> keys;
    List<TValue> values;
    public void OnAfterDeserialize()
    {
        if(keys.Count != values.Count)
        {
            Debug.LogWarning("Key value count mismatch");
        }
        for(int i = 0; i < keys.Count && i < values.Count; i++)
        {
            Add(keys[i], values[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        foreach(var i in this)
        {
            keys.Add(i.Key);
            values.Add(i.Value);
        }
    }
}
