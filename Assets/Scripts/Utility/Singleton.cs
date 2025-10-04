using System.Runtime.CompilerServices;
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
    public static bool HasInstance => _instance != null;

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
                            Debug.Log("make new game object " + typeof(T).Name);
                            GameObject singletonObj = new GameObject(typeof(T).Name);
                            _instance = singletonObj.AddComponent<T>();
                            //DontDestroyOnLoad(singletonObj);
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
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
