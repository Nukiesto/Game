using UnityEngine;

public abstract class MonoGlobalSingleton<T> : MonoBehaviour where T : MonoGlobalSingleton<T>
{
    private static T _instance = null;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;

                if (_instance == null)
                {
                    //Debug.LogWarning("No instance of " + typeof(T).ToString() + ", a temporary one is created.");

                    GameObject go = new GameObject(typeof(T).ToString(), typeof(T));
                    DontDestroyOnLoad(go);

                    _instance = go.GetComponent<T>();

                    if (_instance == null)
                    {
                        Debug.LogError("Problem during the creation of " + typeof(T).ToString());
                    }
                }

                _instance.Init();
            }

            return _instance;
        }
    }

    void Awake()
    {
        Debug.Log(typeof(T).ToString() + "::Awake()");

        if (_instance == null)
        {
            _instance = this as T;
            _instance.Init();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        //Debug.Log(typeof(T).ToString() + "::OnEnable()");

        if (_instance == null)
        {
            _instance = this as T;
        }
    }

    void OnApplicationQuit()
    {
        //Debug.Log(typeof(T).ToString() + "::OnApplicationQuit()");
    }

    void OnDestroy()
    {
        //Debug.Log(typeof(T).ToString() + "::OnDestroy()");

        if (_instance == this)
        {
            _instance = null;
        }
    }

    public virtual void Init() { }
}