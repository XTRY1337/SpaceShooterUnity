using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

public class OptionsManager : MonoBehaviour
{
    public List<Toggle> toggles = new();
    public Toggle realisticBackgroundMoveToggle;
    public Toggle limitLineToggle;
    public Toggle joystick;
    public Slider sliderGameSpeed;
    public Slider sliderVolume;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI volumeText;
    public GameObject panel;
    public GameObject panelNewKey;
    public TextMeshProUGUI currentKey;
    public TextMeshProUGUI waitingKey;
    public TMP_Dropdown dropdown; 

    public GameObject buttonGameSettings;
    private Image _buttonGameSettingsImage;
    private RectTransform _buttonGameSettingsTransform;
    public GameObject panelGameSettings;
    public RectTransform textButtonGame;

    public  GameObject buttonShipSettings;
    private Image _buttonShipSettingsImage;
    private RectTransform _buttonShipSettingsTransform;
    public GameObject panelShipSettings;
    public RectTransform textButtonShip;

    public  GameObject buttonControlsSettings;
    private Image _buttonControlsSettingsImage;
    private RectTransform _buttonControlsSettingsTransform;
    public GameObject panelControlsSettings;
    public GameObject panelControlsSettingsPC;
    public GameObject panelControlsSettingsAndroid;
    public RectTransform textButtonControls;

    private static Color _selectedColor;
    private static Color _normalColor;
    private static int _currentTab;

    public void Start()
    {
        panel.SetActive(false);

        panelGameSettings.SetActive(true); // Active all panels at the begining to get the correct values for toggles
        panelShipSettings.SetActive(true);
        panelControlsSettings.SetActive(true);

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            panelControlsSettingsPC.SetActive(true);
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            panelControlsSettingsAndroid.SetActive(true);

            dropdown.value = SessionManager.GetPlayerMovementControlOption();
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            joystick.isOn = SessionManager.GetJoystick();
            joystick.onValueChanged.AddListener(state =>
            {
                SessionManager.SetJoystick(state);
            });
        }

        float gameSpeed = SessionManager.GetGameSpeed();
        float volume = SessionManager.GetVolume();

        BackgroundMovement.movementOption = 0;

        sliderGameSpeed.value = gameSpeed;
        speedText.text = $"{gameSpeed}x";

        sliderVolume.value = volume;
        volumeText.text = $"{volume}%";

        currentKey.text = SessionManager.GetFireKey().ToString();

        toggles[SessionManager.GetSkin()].isOn = true;

        for (int i = 0; i < toggles.Count; i++)
        {
            int index = i;
            toggles[i].onValueChanged.AddListener(state =>
            {
                if(state)
                {
                    SessionManager.SetSkin(index);
                }
            });
        }

        sliderVolume.onValueChanged.AddListener(val =>
        {
            StartCoroutine(AudioManager.instance.UpdateVolume(val));
        });

        realisticBackgroundMoveToggle.isOn = SessionManager.GetRealistic();
        realisticBackgroundMoveToggle.onValueChanged.AddListener(state =>
        {
            SessionManager.SetRealistic(state);
        });

        limitLineToggle.isOn = SessionManager.GetLimiteLine();
        limitLineToggle.onValueChanged.AddListener(state =>
        {
            SessionManager.SetLimiteLine(state);
        });

        _normalColor = new Color(0.2941f, 0.2941f, 0.2941f, 1f);
        _selectedColor = new Color(0.0716f, 0.0702f, 0.0702f, 1f);

        _buttonGameSettingsImage = buttonGameSettings.GetComponent<Image>();
        _buttonShipSettingsImage = buttonShipSettings.GetComponent<Image>();
        _buttonControlsSettingsImage = buttonControlsSettings.GetComponent<Image>();

        _buttonGameSettingsTransform = buttonGameSettings.GetComponent<RectTransform>();
        _buttonShipSettingsTransform = buttonShipSettings.GetComponent<RectTransform>();
        _buttonControlsSettingsTransform = buttonControlsSettings.GetComponent<RectTransform>();

        _currentTab = 0;

        panelGameSettings.SetActive(true);
        panelShipSettings.SetActive(false);
        panelControlsSettings.SetActive(false);

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            panelControlsSettingsPC.SetActive(false);
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            panelControlsSettingsAndroid.SetActive(false);
        }
    }

    public void OnGameSpeedSliderValueChange()
    {   
        float gameSpeed = (float)Math.Round(sliderGameSpeed.value, 1);
        speedText.text = $"{gameSpeed}x"; 
    }

    public void OnGameSpeedSliderEndDrag()
    {
        SessionManager.SetGameSpeed((float)Math.Round(sliderGameSpeed.value, 1));
    }

    public void OnVolumeSliderValueChange()
    {
        float vol = (float)Math.Round(sliderVolume.value, 0);
        volumeText.text = $"{vol}%";
    }

    public void OnButtonActionYes()
    {
        toggles[0].isOn = true;
        realisticBackgroundMoveToggle.isOn = false;
        limitLineToggle.isOn = false;
        dropdown.value = 0;
        joystick.isOn = false;
        currentKey.text = KeyCode.Mouse0.ToString();
        
        speedText.text = $"{Constants.DefaulGameSpeed}x";
        sliderGameSpeed.value = Constants.DefaulGameSpeed;

        volumeText.text = $"{Constants.DefaulVolume}%";
        sliderVolume.value = Constants.DefaulVolume;

        SessionManager.SetDefaultGameSpeed();
        SessionManager.SetDefaultHighScore();
        SessionManager.SetDefaultSkin();
        SessionManager.SetDefaultVolume();
        SessionManager.SetDefaultRealistic();
        SessionManager.SetDefaultLimitLine();
        SessionManager.SetDefaultPlayerMovementControlOption();
        SessionManager.SetDefaultJoystick();
        SessionManager.SetDefaultFireKey();

        panel.SetActive(false);
    }

    public void OnButtonActionNo()
    {
        panel.SetActive(false);
    }

    public void OnResetButton()
    {
        panel.SetActive(true);
    }

    public void OnVolumeSliderEndDrag()
    {
        SessionManager.SetVolume((int)Math.Round(sliderVolume.value, 0));
    }

    public void OnBackMenuButton()
    {
        panelNewKey.SetActive(false);
        SceneManager.LoadScene(2);
    }

    public void OnGameSettingsButton()
    {
        if(_currentTab == 0)
        {
            return;
        }

        _currentTab = 0;

        _buttonGameSettingsImage.color = _selectedColor;
        _buttonShipSettingsImage.color = _normalColor;
        _buttonControlsSettingsImage.color = _normalColor;

        _buttonGameSettingsTransform.sizeDelta = new Vector2(_buttonGameSettingsTransform.sizeDelta.x, 169.1f);
        _buttonShipSettingsTransform.sizeDelta = new Vector2(_buttonShipSettingsTransform.sizeDelta.x, 116.6f);
        _buttonControlsSettingsTransform.sizeDelta = new Vector2(_buttonControlsSettingsTransform.sizeDelta.x, 116.6f);

        textButtonGame.offsetMax = new Vector2(0f, 65f);
        textButtonShip.offsetMax = new Vector2(0f, 35f);
        textButtonControls.offsetMax = new Vector2(0f, 35f);

        panelGameSettings.SetActive(true);
        panelShipSettings.SetActive(false);
        panelControlsSettings.SetActive(false);
        panelNewKey.SetActive(false);

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            panelControlsSettingsPC.SetActive(false);
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            panelControlsSettingsAndroid.SetActive(false);
        }
    }

    public void OnShipSettingsButton()
    {   
        if(_currentTab == 1)
        {
            return;
        }

        _currentTab = 1;

        _buttonGameSettingsImage.color = _normalColor;
        _buttonShipSettingsImage.color = _selectedColor;
        _buttonControlsSettingsImage.color = _normalColor;

        _buttonGameSettingsTransform.sizeDelta = new Vector2(_buttonGameSettingsTransform.sizeDelta.x, 116.6f);
        _buttonShipSettingsTransform.sizeDelta = new Vector2(_buttonShipSettingsTransform.sizeDelta.x, 169.1f);
        _buttonControlsSettingsTransform.sizeDelta = new Vector2(_buttonControlsSettingsTransform.sizeDelta.x, 116.6f);
        
        textButtonGame.offsetMax = new Vector2(0f, 35f);
        textButtonShip.offsetMax = new Vector2(0f, 65f);
        textButtonControls.offsetMax = new Vector2(0f, 35f);

        panelGameSettings.SetActive(false);
        panelShipSettings.SetActive(true);
        panelControlsSettings.SetActive(false);
        panelNewKey.SetActive(false);

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            panelControlsSettingsPC.SetActive(false);
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            panelControlsSettingsAndroid.SetActive(false);
        }
    }

    public void OnControlSettingsButton()
    {
        if(_currentTab == 2)
        {
            return;
        }

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            panelControlsSettingsPC.SetActive(true);
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            panelControlsSettingsAndroid.SetActive(true);
        }

        _currentTab = 2;    

        _buttonGameSettingsImage.color = _normalColor;
        _buttonShipSettingsImage.color = _normalColor;
        _buttonControlsSettingsImage.color = _selectedColor;

        _buttonGameSettingsTransform.sizeDelta = new Vector2(_buttonGameSettingsTransform.sizeDelta.x, 116.6f);
        _buttonShipSettingsTransform.sizeDelta = new Vector2(_buttonShipSettingsTransform.sizeDelta.x, 116.6f);
        _buttonControlsSettingsTransform.sizeDelta = new Vector2(_buttonControlsSettingsTransform.sizeDelta.x, 169.1f);

        textButtonGame.offsetMax = new Vector2(0f, 35f);
        textButtonShip.offsetMax = new Vector2(0f, 35f);
        textButtonControls.offsetMax = new Vector2(0f, 65f);

        panelGameSettings.SetActive(false);
        panelShipSettings.SetActive(false);
        panelControlsSettings.SetActive(true);
        panelNewKey.SetActive(false);
    }

    private void OnDropdownValueChanged(int value)
    {
        switch (value)
        {
            case 0:
                SessionManager.SetPlayerMovementControlOption(0);
                break;
            case 1:
                SessionManager.SetPlayerMovementControlOption(1);
                break;
        }
    }

    public void OnNewKeyButton()
    {   
        panelNewKey.SetActive(true);
        waitingKey.text = "Waiting Key...";

        StartCoroutine(WaitingNewKey());
    }

    System.Collections.IEnumerator WaitingNewKey()
    {
        while (true)
        {
            Debug.Log(IntroGame.invalidKeys);

            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {   
                if (Input.GetKeyDown(key))
                {
                    if (IntroGame.invalidKeys.Contains(key))
                    {
                        Debug.Log("Tecla inválida pressionada: " + key);
                        waitingKey.color = new Color(a:1, r:255, g:0, b:0);
                        waitingKey.text = "Invalid Key: " + key;
                        yield return new WaitForSeconds(1f);
                        waitingKey.color = new Color(a:1, r:255, g:255, b:255);
                        waitingKey.text = "Waiting Key...";
                        continue;
                    }

                    currentKey.text = key.ToString();
                    SessionManager.SetFireKey(key);
                    panelNewKey.SetActive(false);
                    yield break;
                }
            }

            yield return null;
        }
    }
}
