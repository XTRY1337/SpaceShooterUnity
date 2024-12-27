using UnityEngine;

public class DestroyByContact : MonoBehaviour
{
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _playerExplosion;
    [SerializeField] private Transform _childTransform;

    public static int AsteroidSequence => _asteroidSequence;
    private static int _asteroidSequence;

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

        if(other.tag == "Player")
        {   
            bool gameOver = GameManager.Instance.HandleLifes();
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

        if(!GameManager.IsGameOver)
        {
            GameManager.Instance.AddScore();
        }

        Destroy(other.gameObject); //Destroy shot and player
    }

    private void DestroyAsteroidObject()
    {
        AudioManager.Instance.PlaySoundEffect("ExplosionAsteroid");
        Instantiate(_explosion, _childTransform.position, transform.rotation);
        Destroy(gameObject);
    }

    public static void ResetAsteroidSequence() => _asteroidSequence = 0;
}