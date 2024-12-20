using System.Collections;

using UnityEngine;

public class DestroyByContact : MonoBehaviour
{
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _playerExplosion;
    
    [SerializeField] private int _blinkCount;
    [SerializeField] private float _blinkTime;

    private GameManager _gameController;

    private bool _tutorial;
    private float _gameSpeed;

    void Start()
    {   
        _tutorial = SessionManager.GetFirstPlay();
        _gameSpeed = SessionManager.GetGameSpeed();

        if(_tutorial)
        {
            return;
        }

        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        _gameController = gameControllerObject.GetComponent<GameManager>();
    }

    void OnTriggerEnter(Collider other)
    {   
        //TODO: Fix location of asteroid explosion
        GameObject explosionInstance = Instantiate(_explosion, transform.position, transform.rotation);
        explosionInstance.AddComponent<DestroyByTime>();

        if(other.tag == "Boundary")
        {   
            return;
        }

        if(_tutorial)
        {   
            if(other.tag == "Player")
            {
                CoroutineManager.Instance.StartCoroutine(BlinkEffect());                
            }

            Destroy(gameObject);

            return;
        }

        if(other.tag == "Player")
        {   
            bool gameOver = _gameController.HandleLifes();
            if(!gameOver)
            {
                CoroutineManager.Instance.StartCoroutine(BlinkEffect());
                Destroy(gameObject);
                return;
            }

            GameObject playerExplosionInstance = Instantiate(_playerExplosion, other.transform.position, other.transform.rotation);
            playerExplosionInstance.AddComponent<DestroyByTime>();
        }

        if(!GameManager.gameOver)
        {
            _gameController.AddScore();
        }

        Destroy(other.gameObject);
        Destroy(gameObject);
    }

    public IEnumerator BlinkEffect()
    {   
        GameObject player = GameObject.FindWithTag("Player");
        MeshRenderer playerRenderer = player.GetComponent<MeshRenderer>();
        GameObject engineFire = GameObject.FindWithTag("Fire");

        for (int i = 0; i < _blinkCount; i++)
        {
            playerRenderer.enabled = false;
            engineFire.SetActive(false);
            yield return new WaitForSeconds(_blinkTime / _gameSpeed);
            
            playerRenderer.enabled = true;
            engineFire.SetActive(true);
            yield return new WaitForSeconds(_blinkTime / _gameSpeed);   
        }
    }
}