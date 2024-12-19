using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject _moveKeysAnimator; 
    [SerializeField] private GameObject _mouseClickAnimator; 

    public GameObject closeButton;
    public GameObject leftPanelAndroid;
    public GameObject rightPanelAndroid;
    public GameObject hazard;
    public TextMeshProUGUI title;

    private bool _leftPanelClicked = false;
    private bool _rightPanelClicked = false;

    private HashSet<KeyCode> _tutorialKeys;

    void Start()
    {
        BackgroundMovement.movementOption = 2;
        AudioManager.instance.ChangeMusic(1);
        GameController.gamePaused = false;

        closeButton.SetActive(false);

        if(!SessionManager.GetFirstPlay())
        {   
            closeButton.SetActive(true);
        }

        SessionManager.SetFirstPlay(true);
        StartCoroutine(StartTutorial());

        _tutorialKeys = new HashSet<KeyCode>
        {
            KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D,
            KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow
        };
    }

    IEnumerator StartTutorial()
    {
        GameController.gamePaused = true; //avoid shoot before tutorial start
        GameController.gameSpeed = 0; //avoid ship to nove before tutorial start
        yield return new WaitForSeconds(1.5f);

        //after 1.5 second
        Time.timeScale = 0;
        _moveKeysAnimator.SetActive(true);

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            //While key press
            bool click = false;
            while(!click)
            {
                if (Input.anyKeyDown)
                {
                    foreach (KeyCode key in _tutorialKeys)
                    {
                        if (Input.GetKeyDown(key))
                        {
                            click = true;
                            Time.timeScale = 1;
                            GameController.gameSpeed = 1;
                            _moveKeysAnimator.SetActive(false);
                            yield return null;
                        }
                    }
                    yield return null;
                }
                yield return null;
            }

            Vector3 spawnPosition = VectorCreator.SetVector3(0, 0, 30);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hazard, spawnPosition, spawnRotation); //Send asteroid
            yield return new WaitForSeconds(3f);

            Time.timeScale = 0;
            _mouseClickAnimator.SetActive(true);
            click = false;
            while(!click)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    click = true;
                    Time.timeScale = 1;
                    GameController.gamePaused = false;
                    _mouseClickAnimator.SetActive(false);
                    yield return null;
                }
                
                yield return null;
            }

            spawnPosition = VectorCreator.SetVector3(3, 0, 30);
            Instantiate(hazard, spawnPosition, spawnRotation); //Send asteroid
            yield return new WaitForSeconds(3f);

        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            leftPanelAndroid.SetActive(true);

            while(!_leftPanelClicked)
            {
                yield return null;
            }

            Vector3 spawnPosition = VectorCreator.SetVector3(0, 0, 30);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hazard, spawnPosition, spawnRotation); //Send asteroid
            yield return new WaitForSeconds(3f);

            Time.timeScale = 0;
            rightPanelAndroid.SetActive(true);

            while(!_rightPanelClicked)
            {
                yield return null;
            }

            spawnPosition = VectorCreator.SetVector3(3, 0, 30);
            Instantiate(hazard, spawnPosition, spawnRotation); //Send asteroid
            yield return new WaitForSeconds(3f);
        }

        yield return new WaitForSeconds(3);

        SessionManager.SetFirstPlay(false);
        SceneManager.LoadScene(2);
    }

    public void OnLeftPainelClick()
    {
        leftPanelAndroid.SetActive(false);
        _leftPanelClicked = true;
        Time.timeScale = 1;
        GameController.gameSpeed = 1;
    }

    public void OnRightPainelClick()
    {
        rightPanelAndroid.SetActive(false);
        _rightPanelClicked = true;
        Time.timeScale = 1;
        GameController.gamePaused = false;
    }

    public void OnCloseButton()
    {
        StopCoroutine(StartTutorial());
        Time.timeScale = 1;
        SessionManager.SetFirstPlay(false);
        SceneManager.LoadScene(2);
    }
}
