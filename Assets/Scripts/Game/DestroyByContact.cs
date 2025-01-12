using System.Collections;
using UnityEngine;

public class DestroyByContact : MonoBehaviour
{
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _playerExplosion;
    [SerializeField] private Transform _childTransform;
    [SerializeField] private Renderer _asteroidRenderTwo;
    [SerializeField] private int _blinkCount;
    [SerializeField] private float _blinkTime;

    public static int AsteroidSequence => _asteroidSequence;
    private static int _asteroidSequence;

    private bool _isBlinking;
    private float _gameSpeed;

    public int Lifes;

    void Start()
    {
        _gameSpeed = GameManager.GameSpeed;
    }

    void OnTriggerEnter(Collider other)
    {   
        if(other.tag == "Boundary")
        {   
            return;
        }

        if(TutorialManager.IsTutorial)
        {   
            if(other.tag == "Player")
            {
                CoroutineManager.Instance.StartCoroutine(PlayerTutorialController.Instance.BlinkEffect());
                ResetAsteroidSequence();
                TutorialManager.ResetAsteroidSequence();
            }

            if(TutorialManager.IsWaveTutorialController && other.tag != "Player")
            {
                _asteroidSequence++; 
                TutorialManager.AddOneToAsteroidSequence();
            }

            DestroyAsteroidObject();

            return;
        }

        _asteroidSequence++;
        DestroyAsteroidObject();

        Debug.Log(Lifes);
        
        if(other.tag == "Player")
        {   
            if(PlayerController.IsShieldOn)
            {
                PlayerController.Instance.ChangeShieldState(false);
                return;
            }

            bool gameOver = GameManager.Instance.HandleLifes(tag);
            if(!gameOver)
            {
                CoroutineManager.Instance.StartCoroutine(PlayerController.Instance.BlinkEffect());
                ResetAsteroidSequence();
                GameManager.Instance.ResetMultiplierText();
                return;
            }

            AudioManager.Instance.PlaySoundEffect("ExplosionPlayer");
            Instantiate(_playerExplosion, other.transform.position, other.transform.rotation);
        }

        if(!GameManager.IsGameOver && Lifes <= 0)
        {
            switch(tag)
            {
                case "Asteroid":
                    GameManager.Instance.AddScore();
                    break;
                case "AsteroidTwo":
                    GameManager.Instance.AddScore(valueToAdd: 30);
                    break;
            }
        }

        Destroy(other.gameObject); //Destroy shot and player
    }

    private void DestroyAsteroidObject()
    {
        if(Lifes > 0)
        {
            if(!_isBlinking)
            {
                _isBlinking = true;
                CoroutineManager.Instance.StartCoroutine(BlinkEffectAsteroidTwo());
            }

            Lifes--;
        }

        if(Lifes <= 0)
        {
            AudioManager.Instance.PlaySoundEffect("ExplosionAsteroid");
            Instantiate(_explosion, _childTransform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    private IEnumerator BlinkEffectAsteroidTwo()
    {
        Color originalColor = _asteroidRenderTwo.material.GetColor("_Color"); 
        Color damageColor = new(r: 1, g: 0, b: 0, a: 1);

        for (int i = 0; i < _blinkCount; i++)
        {   
            BlinkAction(damageColor);
            yield return new WaitForSeconds(_blinkTime / _gameSpeed);
            
            BlinkAction(originalColor);
            yield return new WaitForSeconds(_blinkTime / _gameSpeed);   
        }

        _isBlinking = false;
    }

    private void BlinkAction(Color color)
    {   
        try
        {
            _asteroidRenderTwo.material.SetColor("_Color", color);
        }
        catch
        {
            _isBlinking = false;
        }
    }

    public static void ResetAsteroidSequence() => _asteroidSequence = 0;
}