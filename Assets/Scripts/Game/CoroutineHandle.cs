using UnityEngine;

public class CoroutineHandle : MonoBehaviour
{   
    private static CoroutineHandle _instance;
    public static CoroutineHandle Instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
    }
}
