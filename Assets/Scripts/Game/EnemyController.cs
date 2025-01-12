using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject _shot;
    [SerializeField] private Transform _shotSpawn;
    [SerializeField] private float _tilt;
    [SerializeField] private float _speed;
    [SerializeField] private float _xMin, _xMax, _zMin, _zMax;

    private Transform _currentPosition;
    private Transform _targetPosition;
    private float _sinTime;
    private float _gameSpeed;

    void Start()
    {   
        _currentPosition = transform;
        _targetPosition = new GameObject("Target").transform;
        _targetPosition.position = VectorManager.NewVector3(x: Random.Range(_xMin, _xMax), z: 6f);
        _gameSpeed = GameManager.GameSpeed;
    }

    void Update()
    {
        if(GameManager.IsGameOver || GameManager.IsGamePaused)
        {
            return;
        }

        if (Vector3.Distance(transform.position, _targetPosition.position) > 0.1f)
        {
            _sinTime += Time.deltaTime * _speed * _gameSpeed;
            _sinTime = Mathf.Clamp(_sinTime, 0, Mathf.PI);

            float t = Evaluate(_sinTime);
            transform.position = Vector3.Lerp(_currentPosition.position, _targetPosition.position, t);

            //Rotation
            float targetAngle = 0f;
            if (_targetPosition.position.x > transform.position.x) 
            {
                targetAngle = 17f;
            }
            else if (_targetPosition.position.x < transform.position.x) 
            {
                targetAngle = -17f;
            }

            float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * 2f * _gameSpeed);
            transform.rotation = Quaternion.Euler(0, 180, smoothAngle);
        }
        else
        {
            _sinTime = 0f;
            _targetPosition.position = VectorManager.NewVector3(x: Random.Range(_xMin, _xMax), z: Random.Range(_zMin, _zMax));

            Shot();
        }

        if(Vector3.Distance(transform.position, _targetPosition.position) < 1f)
        {
            //Rotation
            float returnAngle = Mathf.LerpAngle(transform.eulerAngles.z, 0f, Time.deltaTime * _tilt * _gameSpeed);
            transform.rotation = Quaternion.Euler(0, 180, returnAngle);
        }
    }
    
    private float Evaluate(float x) => 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;

    private void Shot()
    {
        Instantiate(_shot, _shotSpawn.position, _shotSpawn.rotation);
        AudioManager.Instance.PlaySoundEffect("ShotEnemy");
    }
}
