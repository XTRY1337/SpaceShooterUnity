using UnityEngine;

public class MoveGameObject : MonoBehaviour
{
    [SerializeField] private Rigidbody _objectRb;
    [SerializeField] private float _speed;

    void Start()
    {   
        _objectRb.linearVelocity = transform.forward * _speed * GameManager.GameSpeed;
    }
}
