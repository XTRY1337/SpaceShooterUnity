using UnityEngine;

public class MoveGameObject : MonoBehaviour
{
    [SerializeField] private Rigidbody _objectRb;
    [SerializeField] private float _speed;

    void Start()
    {   
        float gameSpeed;
        
        if(GameManager.gameSpeed == 0) // == 0 when is Intro
        {
            gameSpeed = 1f;
        }
        else
        {
            gameSpeed = GameManager.gameSpeed;
        }

        _objectRb.linearVelocity = transform.forward * _speed * gameSpeed;
    }
}
