using UnityEngine;

public class MoveGameObjectUpgrade : MonoBehaviour
{
    [SerializeField] private Rigidbody _objectRb;
    [SerializeField] private float _speed;

    void Start()
    {   
        _objectRb.linearVelocity = -transform.up * _speed * GameManager.GameSpeed;
    }
}
