using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour Singleton
/// usage£º
/// public class GameManager : Singleton<GameManager> { ... }
/// use GameManager.Instance to access it
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if ( _instance == null )
                        {
                            GameObject singletonObj = new GameObject(typeof(T).Name);
                            _instance = singletonObj.AddComponent<T>();
                            DontDestroyOnLoad(singletonObj);
                        }
                    }
                }

            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if ( _instance == null )
        {
            _instance = this as T;
            //DontDestroyOnLoad(_instance);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
