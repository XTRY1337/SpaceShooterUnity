using System.Collections;
using UnityEngine;

public class EnemyContact : MonoBehaviour
{   
    [SerializeField] private int _blinkCount;
    [SerializeField] private float _blinkTime;
    [SerializeField] private GameObject _enemyExplosion;
    [SerializeField] private Renderer _enemyRenderer;
    private float _gameSpeed;
    private bool _isBlinking;
    public int EnemyLifes;

    void Start()
    {
        _gameSpeed = GameManager.GameSpeed;
    }

    void OnTriggerEnter(Collider other)
    {   
        if(other.tag == "Boundary" || other.tag == "Asteroid")
        {   
            return;
        }

        if(other.tag == "Player")
        {
            Debug.Log(other.tag);
        }

        if(other.tag == "PlayerShot")
        {
            bool gameOver = HandleEnemyLifes();
            if(!gameOver)
            {
                if(!_isBlinking)
                {
                    _isBlinking = true;
                    CoroutineManager.Instance.StartCoroutine(BlinkEnemyEffect());
                }
                Destroy(other.gameObject);
                return;
            }

            AudioManager.Instance.PlaySoundEffect("ExplosionEnemy");
            Instantiate(_enemyExplosion, transform.position, transform.rotation);
            CoroutineManager.Instance.StartCoroutine(GameManager.Instance.EnemyDelay(delay: 5f));
            GameManager.Instance.EnemyDestroyed();

            Destroy(gameObject);
        }
    }

    public IEnumerator BlinkEnemyEffect()
    {   
        Color reflectionOriginalColor = _enemyRenderer.material.GetColor("_ReflectColor"); 
        Color reflectionDamageColor = new(r: 1, g: 0, b: 0, a: 0.5f);

        for (int i = 0; i < _blinkCount; i++)
        {   
            BlinkAction(reflectionDamageColor);
            yield return new WaitForSeconds(_blinkTime / _gameSpeed);
            
            BlinkAction(reflectionOriginalColor);
            yield return new WaitForSeconds(_blinkTime / _gameSpeed);   
        }

        _isBlinking = false;
    }

    private void BlinkAction(Color color)
    {   
        try
        {
            _enemyRenderer.material.SetColor("_ReflectColor", color);
        }
        catch
        {
            _isBlinking = false;
        }
    }

    public bool HandleEnemyLifes()
    {
        EnemyLifes--;

        if(EnemyLifes > 0)
        {
            return false;
        }

        GameManager.Instance.AddScore(valueToAdd: 50);
        return true;
    }
}
