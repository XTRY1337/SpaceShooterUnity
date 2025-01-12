using UnityEngine;

public class EnemyShotContact : MonoBehaviour
{
    [SerializeField] private GameObject _playerExplosion;

    void OnTriggerEnter(Collider other)
    {   
        if(other.tag == "Boundary" || other.tag == "PlayerShot")
        {   
            return;
        }

        if(other.tag == "Player")
        {   
            if(PlayerController.IsShieldOn)
            {
                PlayerController.Instance.ChangeShieldState(false);
                return;
            }

            bool gameOver = GameManager.Instance.HandleLifes(lessScore: 100);
            if(!gameOver)
            {
                CoroutineManager.Instance.StartCoroutine(PlayerController.Instance.BlinkEffect());
                DestroyByContact.ResetAsteroidSequence();
                GameManager.Instance.ResetMultiplierText();
                return;
            }

            AudioManager.Instance.PlaySoundEffect("ExplosionPlayer");
            Destroy(other.gameObject);
            Instantiate(_playerExplosion, other.transform.position, other.transform.rotation);
        }

        Destroy(other.gameObject); //Destroy shoot
    }
}
