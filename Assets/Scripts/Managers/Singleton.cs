using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance => instance;

    public static bool IsInitialized => instance != null;

    [SerializeField][Tooltip("If is pesistant, will go to don't destroy on load.")]
    private bool _isPersistant = false;

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Debug.Log(string.Format("[Singleton] Trying to instantiate a second instance of singleton class {0}", GetType().Name));
            
            if(_isPersistant)
                Destroy(gameObject);
            else
                Destroy(this);
        }
        else
        {
            instance = (T)this;

            if (_isPersistant)
            {
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}