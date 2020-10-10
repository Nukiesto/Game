using UnityEngine;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Check to see if we're about to be destroyed.
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static T m_Instance;

    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (m_ShuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed. Returning null.");
                return null;
            }

            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    // Search for existing instance.
                    m_Instance = (T)FindObjectOfType(typeof(T));

                    // Create new instance if one doesn't already exist.
                    if (m_Instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        // Make instance persistent.
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return m_Instance;
            }
        }
    }


    private void OnApplicationQuit()
    {
        m_ShuttingDown = true;
    }


    private void OnDestroy()
    {
        m_ShuttingDown = true;
    }
}
/*
Чтобы сделать любой класс синглетным, просто унаследуйте его от базового класса Singleton вместо MonoBehaviour, например:

public class MySingleton : Singleton<MySingleton>
{
    //(необязательно) предотвращает использование конструктора, не являющегося синглетным.
    protected MySingleton() { }

    // затем добавьте любой код в класс, который вам нужен, как обычно.
    public string MyTestString = "Hello world!";
}
Теперь вы можете получить доступ ко всем общедоступным полям, свойствам и методам из класса в любом месте с помощью <ClassName>.Пример:

public class MyClass : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log(MySingleton.Instance.MyTestString);
    }
}
*/