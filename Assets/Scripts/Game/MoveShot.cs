using UnityEngine;

public class MoveShot : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    private float _gameSpeed;

    void Start()
    {
        if(GameController.gameSpeed == 0)
        {
            // == 0 when is Intro
            _gameSpeed = 1;
        }
        else
        {
            _gameSpeed = GameController.gameSpeed;
        }

        rb.linearVelocity = transform.forward * speed * _gameSpeed;
    }

    void Update()
    {
        
    }
}
