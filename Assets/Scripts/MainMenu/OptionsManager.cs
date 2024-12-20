using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

public class OptionsManager : MonoBehaviour
{
    [Header("-- Reset Settings --")]
    [SerializeField] private GameObject _resetSettingsPanel;

    [Header("-- Game Settings --")]
    [SerializeField] private Toggle _realisticBackgroundMoveToggle;
    [SerializeField] private Toggle _limitLineToggle;
    [SerializeField] private Slider _gameSpeedSlider;
    [SerializeField] private TextMeshProUGUI _gameSpeedText;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TextMeshProUGUI _volumeText;
    [SerializeField] private GameObject _gameSettingsPanel;
    [SerializeField] private RectTransform _gameSettingsButtonText;
    [SerializeField] private Image _gameSettingsButtonImage;
    [SerializeField] private RectTransform _gameSettingsButtonTransform;

    [Header("-- Ship Settings --")]
    [SerializeField] private List<Toggle> _shipSkinToggles = new();
    [SerializeField] private GameObject _shipSettingsPanel;
    [SerializeField] private RectTransform _shipSettingsButtonText;
    [SerializeField] private Image _shipSettingsButtonImage;
    [SerializeField] private RectTransform _shipSettingsButtonTransform;

    [Header("-- Controls Settings --")]
    [SerializeField] private Toggle _joystickToggle;
    [SerializeField] private TMP_Dropdown _controlsSideDropdown; 
    [SerializeField] private GameObject _newFireKeyPanel;
    [SerializeField] private TextMeshProUGUI _currentKeyText;
    [SerializeField] private TextMeshProUGUI _waitingKeyText;
    [SerializeField] private GameObject _generalControlsSettingsPanel;
    [SerializeField] private GameObject _controlsSettingsPCPanel;
    [SerializeField] private GameObject _controlsSettingsAndroidPanel;
    [SerializeField] private RectTransform _controlsSettingsButtonText;
    [SerializeField] private Image _controlsSettingsButtonImage;
    [SerializeField] private RectTransform _controlsSettingsTransform;

    private RectTransform[] _tabsButtonTransforms = new RectTransform[3];
    private RectTransform[] _tabsButtonTexts = new RectTransform[3];
    private Image[] _tabsButtonImages = new Image[3];
    private GameObject[] _tabsPanels = new GameObject[3];

    private float _defaultSize = 116.6f;
    private float _defaultOffset = 35f;
    private Color _defaultColor = new Color(0.2941f, 0.2941f, 0.2941f, 1f);

    private float _activeSize = 169.1f;
    private float _activeOffset = 65f;
    private Color _activeColor = new Color(0.0716f, 0.0702f, 0.0702f, 1f);

    private bool _isDragging;
    private int _currentTab;

    public void Start()
    {
        BackgroundMovement.MovementOption = 0;

        _currentTab = 0;
        _isDragging = false;

        float gameSpeed = SessionManager.GetGameSpeed();
        float volume = SessionManager.GetVolume();

        _tabsButtonTransforms = new RectTransform[] { _gameSettingsButtonTransform, _shipSettingsButtonTransform, _controlsSettingsTransform };
        _tabsButtonTexts = new RectTransform[] { _gameSettingsButtonText, _shipSettingsButtonText, _controlsSettingsButtonText };
        _tabsButtonImages = new Image[] { _gameSettingsButtonImage, _shipSettingsButtonImage, _controlsSettingsButtonImage };
        _tabsPanels = new GameObject[] { _gameSettingsPanel, _shipSettingsPanel, _generalControlsSettingsPanel };

        _resetSettingsPanel.SetActive(false);

        // Active all panels at the begining to get the correct values for toggles
        _gameSettingsPanel.SetActive(true); 
        _shipSettingsPanel.SetActive(true);
        _generalControlsSettingsPanel.SetActive(true);

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            _controlsSettingsPCPanel.SetActive(true);
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            _controlsSettingsAndroidPanel.SetActive(true);

            _controlsSideDropdown.value = SessionManager.GetPlayerMovementControlOption();
            _controlsSideDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            _joystickToggle.isOn = SessionManager.GetJoystick();
            _joystickToggle.onValueChanged.AddListener(state =>
            {
                SessionManager.SetJoystick(state);
            });
        }

        _gameSpeedSlider.value = gameSpeed;
        _gameSpeedText.text = $"{gameSpeed}x";

        _volumeSlider.value = volume;
        _volumeText.text = $"{volume}%";

        _currentKeyText.text = SessionManager.GetFireKey().ToString();

        _shipSkinToggles[SessionManager.GetSkin()].isOn = true;

        for (int i = 0; i < _shipSkinToggles.Count; i++)
        {
            int index = i;
            _shipSkinToggles[i].onValueChanged.AddListener(state =>
            {
                if(state)
                {
                    SessionManager.SetSkin(index);
                }
            });
        }

        _volumeSlider.onValueChanged.AddListener(val =>
        {
            StartCoroutine(AudioManager.Instance.UpdateVolume(val));
        });

        _realisticBackgroundMoveToggle.isOn = SessionManager.GetRealistic();
        _realisticBackgroundMoveToggle.onValueChanged.AddListener(state =>
        {
            SessionManager.SetRealistic(state);
        });

        _limitLineToggle.isOn = SessionManager.GetLimiteLine();
        _limitLineToggle.onValueChanged.AddListener(state =>
        {
            SessionManager.SetLimiteLine(state);
        });

        // Active the first panel
        _gameSettingsPanel.SetActive(true);
        _shipSettingsPanel.SetActive(false);
        _generalControlsSettingsPanel.SetActive(false);

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            _controlsSettingsPCPanel.SetActive(false);
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            _controlsSettingsAndroidPanel.SetActive(false);
        }
    }

    #region General
    public void OnButtonActionYes()
    {
        _shipSkinToggles[0].isOn = true;
        _realisticBackgroundMoveToggle.isOn = false;
        _limitLineToggle.isOn = false;
        _controlsSideDropdown.value = 0;
        _joystickToggle.isOn = false;
        _currentKeyText.text = KeyCode.Mouse0.ToString();
        
        _gameSpeedText.text = $"{Constants.DefaulGameSpeed}x";
        _gameSpeedSlider.value = Constants.DefaulGameSpeed;

        _volumeText.text = $"{Constants.DefaulVolume}%";
        _volumeSlider.value = Constants.DefaulVolume;

        SessionManager.SetDefaultGameSpeed();
        SessionManager.SetDefaultHighScore();
        SessionManager.SetDefaultSkin();
        SessionManager.SetDefaultVolume();
        SessionManager.SetDefaultRealistic();
        SessionManager.SetDefaultLimitLine();
        SessionManager.SetDefaultPlayerMovementControlOption();
        SessionManager.SetDefaultJoystick();
        SessionManager.SetDefaultFireKey();

        _resetSettingsPanel.SetActive(false);
    }

    public void OnButtonActionNo()
    {
        _resetSettingsPanel.SetActive(false);
    }

    public void OnResetButton()
    {
        _resetSettingsPanel.SetActive(true);
    }

    public void OnBackMenuButton()
    {
        _newFireKeyPanel.SetActive(false);
        SceneManager.LoadScene(2);
    }
    
    private void TabsController()
    {
        for (int i = 0; i < _tabsButtonTransforms.Length; i++)
        {
            bool isActive = i == _currentTab;

            _tabsButtonTransforms[i].sizeDelta = new Vector2(_tabsButtonTransforms[i].sizeDelta.x, isActive ? _activeSize : _defaultSize);
            _tabsButtonTexts[i].offsetMax = new Vector2(0f, isActive ? _activeOffset : _defaultOffset);
            _tabsButtonImages[i].color = isActive ? _activeColor : _defaultColor;
            _tabsPanels[i].SetActive(isActive);
        }

        _newFireKeyPanel.SetActive(false);
    }
    #endregion
  
    #region GameSettings
    public void OnGameSettingsButton()
    {
        if(_currentTab == 0)
        {
            return;
        }

        _currentTab = 0;

        TabsController();

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            _controlsSettingsPCPanel.SetActive(false);
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            _controlsSettingsAndroidPanel.SetActive(false);
        }
    }

    public void OnGameSpeedSliderValueChange()
    {   
        float gameSpeed = (float)Math.Round(_gameSpeedSlider.value, 1);
        _gameSpeedText.text = $"{gameSpeed}x"; 

        if(!_isDragging)
        {
            SessionManager.SetGameSpeed(gameSpeed);
        }
    }

    public void OnGameSpeedSliderBeginDrag()
    {
        _isDragging = true;
    }

    public void OnGameSpeedSliderEndDrag()
    {   
        _isDragging = false;
        float gameSpeed = (float)Math.Round(_gameSpeedSlider.value, 1);
        SessionManager.SetGameSpeed(gameSpeed);
    }

    public void OnVolumeSliderValueChange()
    {
        float volume = (float)Math.Round(_volumeSlider.value, 0);
        _volumeText.text = $"{volume}%";
    }

    public void OnVolumeSliderEndDrag()
    {
        int volume = (int)Math.Round(_volumeSlider.value, 0);
        SessionManager.SetVolume(volume);
    }
    #endregion

    #region ShipSettings
    public void OnSkinImageClick(int index)
    {
        Debug.Log("sk");
        for (int j = 0; j < _shipSkinToggles.Count; j++)
        {
            _shipSkinToggles[j].isOn = false;
        }
        _shipSkinToggles[index].isOn = true;
    }

    public void OnShipSettingsButton()
    {   
        if(_currentTab == 1)
        {
            return;
        }

        _currentTab = 1;

        TabsController();

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            _controlsSettingsPCPanel.SetActive(false);
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            _controlsSettingsAndroidPanel.SetActive(false);
        }
    }
    #endregion

    #region ControlsSettings
    public void OnControlSettingsButton()
    {
        if(_currentTab == 2)
        {
            return;
        }

        _currentTab = 2;   

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            _controlsSettingsPCPanel.SetActive(true);
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            _controlsSettingsAndroidPanel.SetActive(true);
        }

        TabsController();
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
        _newFireKeyPanel.SetActive(true);
        _waitingKeyText.text = "Waiting Key...";

        StartCoroutine(WaitingNewKey());
    }

    System.Collections.IEnumerator WaitingNewKey()
    {
        while (true)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {   
                if (Input.GetKeyDown(key))
                {
                    if (GameIntroduction.InvalidRemappingKeys.Contains(key))
                    {
                        _waitingKeyText.color = new Color(a:1, r:255, g:0, b:0);
                        _waitingKeyText.text = "Invalid Key: " + key;
                        yield return new WaitForSeconds(1f);
                        _waitingKeyText.color = new Color(a:1, r:255, g:255, b:255);
                        _waitingKeyText.text = "Waiting Key...";
                        continue;
                    }

                    _currentKeyText.text = key.ToString();
                    SessionManager.SetFireKey(key);
                    _newFireKeyPanel.SetActive(false);
                    yield break;
                }
            }

            yield return null;
        }
    }
    #endregion
}
