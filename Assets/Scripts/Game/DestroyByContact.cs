using System.Collections;
using UnityEngine;

public class DestroyByContact : MonoBehaviour
{
    public GameObject explosion;
    public GameObject playerExplosion;
    private GameObject _player;

    public int scoreValue;
    public int blinkCount;
    public float blinkTime;

    private GameController _gameController;
    private bool _tutorial;

    void Start()
    {   
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if(gameControllerObject != null)
        {
            _gameController = gameControllerObject.GetComponent<GameController>();
        }
        if(_gameController == null)
        {
            Debug.Log("Can't find GameController script");
        }

        _player = PlayerController.player;
        _tutorial = SessionManager.GetFirstPlay();
    }

    void OnTriggerEnter(Collider other)
    {   
        Debug.Log(_tutorial);
        if(_tutorial)
        {   

            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }

        if(other.tag == "Boundary")
        {   
            return;
        }

        //Fix location of asteroid explosion
        GameObject explosionInstance = Instantiate(explosion, transform.position, transform.rotation);
        explosionInstance.AddComponent<DestroyByTime>();

        if(other.tag == "Player")
        {   
            bool gameOver = _gameController.HandleLifes();
            if(!gameOver)
            {
                CoroutineHandle.Instance.StartCoroutine(BlinkEffect());
                Destroy(gameObject);
                return;
            }

            GameObject playerExplosionInstance = Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
            playerExplosionInstance.AddComponent<DestroyByTime>();
        }

        if(!GameController.gameOver)
        {
            _gameController.AddScore();
        }

        Destroy(other.gameObject);
        Destroy(gameObject);
    }

    private IEnumerator BlinkEffect()
    {
        MeshRenderer playerRenderer = _player.GetComponent<MeshRenderer>();
        GameObject fireObject = GameObject.FindWithTag("Fire");

        for (int i = 0; i < blinkCount; i++)
        {
            if(playerRenderer == null)
            {
                yield break;
            }

            playerRenderer.enabled = false;
            fireObject.SetActive(false);
            yield return new WaitForSeconds(blinkTime);
            
            playerRenderer.enabled = true;
            fireObject.SetActive(true);
            yield return new WaitForSeconds(blinkTime);   
        }
    }
}