using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private List<Animation> _scoreAnimation = new(); 

    public PlayerController playerController;
    public GameObject hazard;
    public GameObject restartButton;
    public GameObject menuButton;
    public GameObject panel;

    public Slider sliderVolume;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScore;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI newScoreInfo;
    public TextMeshProUGUI volumeText;

    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;
    public float spawnXMin;
    public float spawnXMax;
    public float spawmZ;

    public static bool gameOver;
    public static float gameSpeed;

    private int _bestScoreSaver;
    private int _score;
    private bool _restart;

    public int health = 3;
    public List<Image> hearts = new();
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public List<Sprite> pauseStartImg = new();
    public Image buttonImage;
    public Button pauseButton; 

    public static bool gamePaused;

    void Start()
    {
        gameOver = false;
        gameOverText.text = "";

        _restart = false;
        restartButton.SetActive(false);

        menuButton.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        _score = 0;
        scoreText.text = "";
        WriteScore();

        newScoreInfo.text = "";

        _bestScoreSaver = SessionManager.GetHighScore();
        bestScore.text = $"Best Score: {_bestScoreSaver}";

        gameSpeed = SessionManager.GetGameSpeed();
        panel.SetActive(false);

        float volume = SessionManager.GetVolume();
        sliderVolume.value = volume;
        volumeText.text = $"{volume}%";

        gamePaused = false;

        BackgroundMovement.movementOption = 1;

        sliderVolume.onValueChanged.AddListener(val =>
        {
            StartCoroutine(AudioManager.instance.UpdateVolume(val));
        });

        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        foreach(var img in hearts)
        {
            img.sprite = emptyHeart;
        }

        for(int i = 0; i < health; i++)
        {
            hearts[i].sprite = fullHeart;
        }
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait / gameSpeed);

        while(!gameOver)
        {
            for(int i = 0; i < hazardCount; i++)
            {
                if(gameOver)
                {   
                    break;
                }

                Vector3 spawnPosition = VectorCreator.SetVector3(UnityEngine.Random.Range(spawnXMin, spawnXMax), 0, spawmZ);
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(hazard, spawnPosition, spawnRotation);

                yield return new WaitForSeconds(spawnWait / gameSpeed);
            }

            yield return new WaitForSeconds(waveWait / gameSpeed);
        }
    }

    public void GameOver()
    {
        StopCoroutine(SpawnWaves());

        gameOver = true;
        gameOverText.text = "Game Over";

        _restart = true;
        restartButton.SetActive(true);
        menuButton.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        playerController.ClearJoystick();

        SessionManager.SetHighScore(_score);
    }

    public bool HandleLifes()
    {
        health--;
        if(health > 0)
        {
            int saveDiference = _score - Constants.RemoveScore;
            _score -= Constants.RemoveScore;
            newScoreInfo.text = $" -{Constants.RemoveScore}";

            if(_score < 0)
            {   
                if(_score == -Constants.RemoveScore)
                {   
                    newScoreInfo.text = string.Empty;          
                }
                else
                {
                    newScoreInfo.text = $" {saveDiference}";
                }
                _score = 0;
            }
                
            WriteScore();
            
            _scoreAnimation[0].Play("ScoreDownWithVanish");
            _scoreAnimation[2].Play("Lifes");

            return false;
        }

        _scoreAnimation[2].Play("Lifes");
        GameOver();
        return true;
    }

    public void OnRestartButton()
    {
        if(_restart)
        {   
            SceneManager.LoadScene(4);
        }
    }

    public void AddScore()
    {
        _score += Constants.AddScore;
        WriteScore();
        newScoreInfo.text = $" +{Constants.AddScore}";

        _scoreAnimation[0].Play("ScoreUpWithVanish");
        
        if(_score > _bestScoreSaver)
        {
            _bestScoreSaver = _score;
            WriteBestScore();
            _scoreAnimation[1].Play("ScoreUp");
        }
    }

    public void WriteScore()
    {
        scoreText.text = $"Score: {_score}";
    }

    public void WriteBestScore()
    {
        bestScore.text = $"Best Score: {_bestScoreSaver}";
    }

    public void OnPauseButton()
    {
        Time.timeScale = 0;
        gamePaused = true;
        panel.SetActive(true);
        playerController.ClearJoystick();
        buttonImage.sprite = pauseStartImg[0];
        pauseButton.onClick.RemoveAllListeners();
        pauseButton.onClick.AddListener(OnPlayButton);
    }

    public void OnPlayButton()
    {
        Time.timeScale = 1;
        gamePaused = false;
        panel.SetActive(false);
        buttonImage.sprite = pauseStartImg[1];
        pauseButton.onClick.RemoveAllListeners();
        pauseButton.onClick.AddListener(OnPauseButton);
    }

    public void OnVolumeSliderValueChange()
    {
        float vol = (float)Math.Round(sliderVolume.value, 0);
        volumeText.text = $"{vol}%";
    }

    public void OnVolumeSliderEndDrag()
    {
        SessionManager.SetVolume((int)Math.Round(sliderVolume.value, 0));
    }
}