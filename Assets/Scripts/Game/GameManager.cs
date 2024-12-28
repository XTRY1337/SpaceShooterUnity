using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("-- Game State --")]
    [SerializeField] private GameObject _restartButton;
    [SerializeField] private GameObject _menuButton;
    [SerializeField] private TextMeshProUGUI _gameOverText;

    [Header("-- Score/Lifes/Shield --")]
    [SerializeField] private List<Animation> _scoreAnimation = new(); 
    [SerializeField] private List<Image> _heartsInGame = new();
    [SerializeField] private Sprite _fullHeartSprite;
    [SerializeField] private Sprite _emptyHeartSprite;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _bestScoreText;
    [SerializeField] private TextMeshProUGUI _newScoreInfoText;
    [SerializeField] private TextMeshProUGUI _multiplierText;
    [SerializeField] private GameObject _shield;
    [SerializeField] private Image _shieldImage;

    [Header("-- Pause Menu --")]
    [SerializeField] private List<Sprite> _pauseStartSprite = new();
    [SerializeField] private PlayerController _playerController;    
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Button _pauseButton; 
    [SerializeField] private TextMeshProUGUI _volumeText;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Image _pauseButtonImage;

    [Header("-- Wave Parameters --")]
    [SerializeField] private GameObject _hazard;
    [SerializeField] private int _hazardsPerWave;
    [SerializeField] private float _spawnWaitTime;
    [SerializeField] private float _startWaitTime;
    [SerializeField] private float _spawnXMin;
    [SerializeField] private float _spawnXMax;
    [SerializeField] private float _spawmZ;

    private int _shieldSpawnThreshold = 0;
    private bool _isFirstWave;
    private int _playerHealth;
    private int _bestScore;
    private int _score;
    private int _volume;
    private static float _spawnZUpgrade;

    public static float GameSpeed => _gameSpeed;
    private static float _gameSpeed = Constants.DefaulGameSpeed;

    public static bool IsGamePaused => _isGamePaused;
    private static bool _isGamePaused;

    public static bool IsGameOver => _isGameOver;
    private static bool _isGameOver;

    private static GameManager _instance;
    public static GameManager Instance => _instance;

    void Start()
    {
        _instance = this;

        BackgroundMovement.MovementOption = 1;

        DestroyByContact.ResetAsteroidSequence();

        _score = 0;
        _playerHealth = 3;
        _isFirstWave = true;
        _isGamePaused = false;
        _isGameOver = false;
        _bestScore = SessionManager.GetHighScore();
        _gameSpeed = SessionManager.GetGameSpeed();
        _volume = SessionManager.GetVolume();
        _volumeSlider.value = _volume;
        _spawnZUpgrade = _spawmZ - 7;
        
        _gameOverText.text = "";
        _scoreText.text = "";
        _newScoreInfoText.text = "";   

        WriteVolume();
        WriteBestScore();
        WriteScore();

        _restartButton.SetActive(false);
        _menuButton.SetActive(false);
        _pauseButton.gameObject.SetActive(true);
        _pausePanel.SetActive(false);

        StartCoroutine(SpawnWaves());
    }

    #region GameEvents
    IEnumerator SpawnWaves()
    {
        if(_isFirstWave)
        {
            _isFirstWave = false;
            yield return new WaitForSeconds(_startWaitTime / GameSpeed);
        }

        while(!IsGameOver)
        {
            if(true)//_score >= 0 && _score < 150) // Level 1 / Stage 1
            {
                for(int i = 0; i < _hazardsPerWave; i++)
                {
                    if(IsGameOver)
                    {   
                        break;
                    }

                    //Asteroid
                    Vector3 spawnPosition = VectorManager.NewVector3(UnityEngine.Random.Range(_spawnXMin, _spawnXMax), 0, _spawmZ);
                    Quaternion spawnRotation = Quaternion.identity;
                    Instantiate(_hazard, spawnPosition, spawnRotation);

                    // Caso o shield não esteja ativo
                    if (!PlayerController.IsShieldOn)
                    {
                        // Configura o próximo threshold se não estiver configurado ou foi resetado
                        if (_shieldSpawnThreshold == 0)
                        {
                            _shieldSpawnThreshold = _score + 100; // Começa a contagem de 100 pontos a partir do score atual
                        }

                        // Verifica se atingiu ou ultrapassou o threshold
                        if (_score >= _shieldSpawnThreshold)
                        {
                            // Spawna o shield
                            Vector3 spawnShield = VectorManager.NewVector3(UnityEngine.Random.Range(_spawnXMin, _spawnXMax), 0, _spawnZUpgrade);
                            Instantiate(_shield, spawnShield, _shield.transform.rotation);

                            // Reseta o threshold para evitar múltiplos spawns
                            _shieldSpawnThreshold = 0;
                        }
                    }
                    else
                    {
                        // Reseta o threshold enquanto o shield está ativo
                        _shieldSpawnThreshold = 0;
    }

                    yield return new WaitForSeconds(_spawnWaitTime / GameSpeed);
                }
            }
            /*else if(_score >= 150 && _score < 400)
            {
                Debug.Log("Level 2");
            }*/
        }
    }
    
    private void UpdateHearts()
    {
        for (int i = 0; i < _heartsInGame.Count; i++)
        {
            _heartsInGame[i].sprite = i < _playerHealth ? _fullHeartSprite : _emptyHeartSprite;
        }
    }

    public bool HandleLifes()
    {
        _playerHealth--;

        UpdateHearts();

        if(_playerHealth > 0)
        {
            int saveDiference = _score - Constants.RemoveScore;
            _score -= Constants.RemoveScore;
            _newScoreInfoText.text = $" -{Constants.RemoveScore}";

            if(_score < 0)
            {   
                if(_score == -Constants.RemoveScore)
                {   
                    _newScoreInfoText.text = string.Empty;          
                }
                else
                {
                    _newScoreInfoText.text = $" {saveDiference}";
                }
                _score = 0;
            }
                
            WriteScore();
            
            _scoreAnimation[0].Play("ScoreDownWithVanish");
            _scoreAnimation[2].Play("Lifes");

            return false;
        }

        _scoreAnimation[2].Play("Lifes");
        OnGameOver();

        return true;
    }
    
    public void ChangeShieldStateUI(bool enable)
    {
        _shieldImage.enabled = enable;
    }
    #endregion

    #region GameController
    public void OnGameOver()
    {
        StopCoroutine(SpawnWaves());

        _isGameOver = true;
        _gameOverText.text = "Game Over";

        _restartButton.SetActive(true);
        _menuButton.SetActive(true);
        _pauseButton.gameObject.SetActive(false);
        _playerController.ClearJoystick();

        SessionManager.SetHighScore(_score);
    }
    
    public void OnRestartButton()
    {
        SceneManager.LoadScene(4);
    }
    #endregion

    #region Score
    public void AddScore()
    {
        int valueToAdd = Constants.AddScore;

        if(DestroyByContact.AsteroidSequence >= 5 && DestroyByContact.AsteroidSequence < 15)
        {
            //x2
            valueToAdd *= 2;

            if(_multiplierText.text != "x2")
            {
                _multiplierText.text = "x2";
                _scoreAnimation[3].Play("MultiplierOn");
            }

        }
        else if(DestroyByContact.AsteroidSequence >= 15)
        {
            //x3
            valueToAdd *= 3;

            if(_multiplierText.text != "x3")
            {
                _multiplierText.text = "x3";
                _scoreAnimation[3].Play("MultiplierOn");
            }
        }

        _score += valueToAdd;
        _newScoreInfoText.text = $" +{valueToAdd}";

        WriteScore();
        _scoreAnimation[0].Play("ScoreUpWithVanish");
        
        if(_score > _bestScore)
        {
            _bestScore = _score;

            WriteBestScore();
            _scoreAnimation[1].Play("ScoreUp");
        }
    }

    public void ResetMultiplierText()
    {
        _scoreAnimation[3].Play("MultiplierOff");
        StartCoroutine(ClearMultiplierTextAfterDelay(1f));
    }
    
    private IEnumerator ClearMultiplierTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _multiplierText.text = "";
    }

    private void WriteScore()
    {
        _scoreText.text = $"Score: {_score}";
    }

    private void WriteBestScore()
    {
        _bestScoreText.text = $"Best Score: {_bestScore}";
    }
    #endregion

    #region PauseMenu
    public void OnPauseButton()
    {   
        _pauseButtonImage.sprite = _pauseStartSprite[0];
        Time.timeScale = 0;
        _isGamePaused = true;

        _pausePanel.SetActive(true);
        _playerController.ClearJoystick();
        _pauseButton.onClick.RemoveAllListeners();
        _pauseButton.onClick.AddListener(OnPlayButton);
    }

    public void OnPlayButton()
    {   
        _pauseButtonImage.sprite = _pauseStartSprite[1];
        Time.timeScale = 1;
        _isGamePaused = false;

        _pausePanel.SetActive(false);
        _pauseButton.onClick.RemoveAllListeners();
        _pauseButton.onClick.AddListener(OnPauseButton);
    }

    public void OnVolumeSliderValueChange()
    {
        _volume = (int)Math.Round(_volumeSlider.value, 0);
        StartCoroutine(AudioManager.Instance.UpdateVolume(_volume));
        WriteVolume();
    }

    public void OnVolumeSliderEndDrag()
    {
        _volume = (int)Math.Round(_volumeSlider.value, 0);
        SessionManager.SetVolume(_volume);
    }
    
    private void WriteVolume()
    {
        _volumeText.text = $"{_volume}%";
    }

    public void OnBackMenuButton()
    {   
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }

        AudioManager.Instance.SetNewMusic(0);
        SceneManager.LoadScene(2);
    }
    #endregion
}