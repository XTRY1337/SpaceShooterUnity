using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _finishTutorialMsg;
    [SerializeField] private Animation _finishTutorialMsgAnimation;
    [SerializeField] private GameObject _moveKeysAnimator; 
    [SerializeField] private GameObject _mouseClickAnimator; 
    [SerializeField] private GameObject _closeButton;
    [SerializeField] private GameObject _leftPanelAndroid;
    [SerializeField] private GameObject _rightPanelAndroid;
    [SerializeField] private GameObject _hazard;
    [SerializeField] private GameObject _handleObjective;

    [SerializeField] private float _spawnXMin;
    [SerializeField] private float _spawnXMax;
    [SerializeField] private float _spawmZ;
    [SerializeField] private float _spawnWaitTime;

    private static Animation _localObjectiveAnimation;
    private bool _leftPanelClicked;
    private bool _rightPanelClicked;
    private static bool _isStageMovePlayer;
    private static bool _isStageFire;
    private bool _isWindows;
    private static int _asteroidObjectiveCounter;
    private static bool _isWaveTutorialController;

    public static TextMeshProUGUI _objectiveValue;
    public static bool IsWaveTutorialController => _isWaveTutorialController;
    public static bool IsStageMovePlayer => _isStageMovePlayer;
    public static bool IsStageFire => _isStageFire;

    void Start()
    {   
        _leftPanelClicked = false;
        _rightPanelClicked = false;
        _isStageMovePlayer = false;
        _isStageFire = false;
        _isWaveTutorialController = false;
        _isWindows = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
        _asteroidObjectiveCounter = 0;

        GameObject objectiveText = GameObject.FindWithTag("Objective");
        _objectiveValue = objectiveText.GetComponent<TextMeshProUGUI>();

        GameObject objectiveArea = GameObject.FindWithTag("ObjectiveArea");
        _localObjectiveAnimation = objectiveArea.GetComponent<Animation>();

        _handleObjective.SetActive(false);
        _finishTutorialMsg.gameObject.SetActive(false);

        BackgroundMovement.MovementOption = 2;

        AudioManager.Instance.SetNewMusic(1);

        _closeButton.SetActive(false);

        if(!SessionManager.GetFirstPlay())
        {   
            _closeButton.SetActive(true);
        }

        SessionManager.SetFirstPlay(true);

        StartCoroutine(ExecuteTutorialWithWaves());
    }

    IEnumerator ExecuteTutorialWithWaves()
    {
        yield return StartCoroutine(StartTutorial());
        yield return StartCoroutine(SpawnTutorialWaves());
    }

    IEnumerator StartTutorial()
    {   
        yield return new WaitForSeconds(1.5f);

        //after 1.5 second
        Time.timeScale = 0;

        if(_isWindows)
        {   
            _moveKeysAnimator.SetActive(true);

            while(!_isStageMovePlayer) //While key not pressed
            {
                if (Input.anyKeyDown)
                {
                    foreach (KeyCode key in GameIntroduction.TutorialAvailableKeys)
                    {
                        if (Input.GetKeyDown(key))
                        {
                            _isStageMovePlayer = true;
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

            while(!_isStageFire)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    _isStageFire = true;
                    _mouseClickAnimator.SetActive(false);

                    Time.timeScale = 1;

                    yield return null;
                }
                
                yield return null;
            }
        }
        else
        {
            _leftPanelAndroid.SetActive(true);
            _isStageMovePlayer = true;

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

            _isStageFire = true;
        }

        yield return new WaitForSeconds(3f);

        _handleObjective.SetActive(true);
        _localObjectiveAnimation.Play("ShowText");

        yield return new WaitForSeconds(1f);
    }

    IEnumerator SpawnTutorialWaves()
    {
        _isWaveTutorialController = true;

        while(true)
        {   
            if (_asteroidObjectiveCounter >= 3)
            {
                break;
            }

            Vector3 spawnPosition = VectorManager.NewVector3(Random.Range(_spawnXMin, _spawnXMax), 0, _spawmZ);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(_hazard, spawnPosition, spawnRotation);

            yield return new WaitForSeconds(_spawnWaitTime);
        }
        
        _isWaveTutorialController = false;

        _finishTutorialMsg.gameObject.SetActive(true);
        _finishTutorialMsgAnimation.Play("FinishTutorial");

        yield return new WaitForSeconds(2f);

        SessionManager.SetFirstPlay(false);
        SceneManager.LoadScene(2);
    }

    public static void SetNewAsteroidCounter()
    {
        _localObjectiveAnimation.Play("TutorialObjective");
        _asteroidObjectiveCounter++;
        _objectiveValue.text = $"{_asteroidObjectiveCounter} / 3";
    }

    public static void ResetAsteroidCounter()
    {
        _localObjectiveAnimation.Play("TutorialObjectiveFail");
        _asteroidObjectiveCounter = 0;
        _objectiveValue.text = $"{_asteroidObjectiveCounter} / 3";
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
