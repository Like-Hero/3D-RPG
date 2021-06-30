using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _ins;
    public static T Ins { get { return _ins; } }

    protected virtual void Awake()
    {
        if (_ins != null) Destroy(gameObject);
        else _ins = (T)this;
    }
    protected virtual void OnDestroy()
    {
        _ins = null;
    }
    public static bool IsInitialized
    {
        get { return _ins != null; }
    }
}
