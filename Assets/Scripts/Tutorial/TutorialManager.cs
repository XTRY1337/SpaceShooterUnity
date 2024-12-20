using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject _moveKeysAnimator; 
    [SerializeField] private GameObject _mouseClickAnimator; 
    [SerializeField] private GameObject _closeButton;
    [SerializeField] private GameObject _leftPanelAndroid;
    [SerializeField] private GameObject _rightPanelAndroid;
    [SerializeField] private GameObject _hazard;

    private bool _leftPanelClicked;
    private bool _rightPanelClicked;
    private static bool _stageMovePlayer;
    private static bool _stageFire;

    public static bool StageMovePlayer => _stageMovePlayer;
    public static bool StageFire => _stageFire;

    void Start()
    {
        _leftPanelClicked = false;
        _rightPanelClicked = false;
        _stageMovePlayer = false;
        _stageFire = false;

        BackgroundMovement.MovementOption = 2;

        AudioManager.Instance.SetNewMusic(1);
        
        _closeButton.SetActive(false);

        if(!SessionManager.GetFirstPlay())
        {   
            _closeButton.SetActive(true);
        }

        SessionManager.SetFirstPlay(true);

        StartCoroutine(StartTutorial());
    }

    IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(1.5f);

        //after 1.5 second
        Time.timeScale = 0;

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            _moveKeysAnimator.SetActive(true);

            while(!_stageMovePlayer) //While key not pressed
            {
                if (Input.anyKeyDown)
                {
                    foreach (KeyCode key in GameIntroduction.TutorialAvailableKeys)
                    {
                        if (Input.GetKeyDown(key))
                        {
                            _stageMovePlayer = true;
                            _moveKeysAnimator.SetActive(false);

                            Time.timeScale = 1;

                            yield return null;
                        }
                    }

                    yield return null;
                }

                yield return null;
            }

            yield return new WaitForSeconds(3f);

            //Send asteroid
            Vector3 spawnPosition = VectorManager.NewVector3(0, 0, 17);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(_hazard, spawnPosition, spawnRotation); 

            yield return new WaitForSeconds(.4f);

            Time.timeScale = 0;

            _mouseClickAnimator.SetActive(true);

            while(!_stageFire)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    _stageFire = true;
                    _mouseClickAnimator.SetActive(false);

                    Time.timeScale = 1;

                    yield return null;
                }
                
                yield return null;
            }

            yield return new WaitForSeconds(3f);

            spawnPosition = VectorManager.NewVector3(3, 0, 17);
            Instantiate(_hazard, spawnPosition, spawnRotation);

            //TODO: handle text animation saying, try again or good joob, until player destroy at least 2 consecutive asteroides
                //Aparecer um texto a dizer objectivo: 0/2(se sair fora do mapa o asteroid sem ser destruido nao fazer nada)
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            _leftPanelAndroid.SetActive(true);
            _stageMovePlayer = true;

            while(!_leftPanelClicked)
            {
                yield return null;
            }

            yield return new WaitForSeconds(3f);

            //Send asteroid
            Vector3 spawnPosition = VectorManager.NewVector3(0, 0, 17);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(_hazard, spawnPosition, spawnRotation); 

            yield return new WaitForSeconds(.4f);

            Time.timeScale = 0;

            _rightPanelAndroid.SetActive(true);

            while(!_rightPanelClicked)
            {
                yield return null;
            }

            _stageFire = true;

            yield return new WaitForSeconds(3f);

            spawnPosition = VectorManager.NewVector3(3, 0, 17);
            Instantiate(_hazard, spawnPosition, spawnRotation);
        }

        yield return new WaitForSeconds(4);

        SessionManager.SetFirstPlay(false);
        SceneManager.LoadScene(2);
    }

    public void OnLeftPainelClick()
    {
        _leftPanelAndroid.SetActive(false);
        _leftPanelClicked = true;
        Time.timeScale = 1;
    }

    public void OnRightPainelClick()
    {
        _rightPanelAndroid.SetActive(false);
        _rightPanelClicked = true;
        Time.timeScale = 1;
    }

    public void OnCloseButton()
    {
        Time.timeScale = 1;

        SessionManager.SetFirstPlay(false);
        SceneManager.LoadScene(2);
    }
}
