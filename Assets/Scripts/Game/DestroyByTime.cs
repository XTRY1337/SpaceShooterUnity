using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    void Start()
    {
        float lifeTime = 2f;
        Destroy(gameObject, lifeTime);
    }
}
