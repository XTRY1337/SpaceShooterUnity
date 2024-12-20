using UnityEngine;

public class CoroutineManager : MonoBehaviour
{   
    private static CoroutineManager _instance;
    public static CoroutineManager Instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
