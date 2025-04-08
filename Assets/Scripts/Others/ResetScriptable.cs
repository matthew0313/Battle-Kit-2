using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ResetScriptable : ScriptableObject
{
    static readonly List<ResetScriptable> instances = new();
    protected abstract void OnReset();
    private void OnEnable()
    {
        instances.Add(this);
    }
    private void OnDisable()
    {
        instances.Remove(this);
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetInstances()
    {
        foreach (var i in instances)
        {
            i.OnReset();
            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => { i.OnReset(); };
        }
    }
}