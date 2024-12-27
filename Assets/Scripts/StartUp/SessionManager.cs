using System.IO;

using UnityEngine;

using Newtonsoft.Json;

public class SessionManager : MonoBehaviour
{
    private static string _sessionFilePath;
    private static bool _isWindows;

    void Start()
    {
        _sessionFilePath = Path.Combine(Application.persistentDataPath, Constants.SessionFileName);

        if (!File.Exists(_sessionFilePath))
        {   
            _isWindows = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
            CreateDefaulSession();
        }

        int graphicsLevel = GetGraphics();
        QualitySettings.SetQualityLevel(graphicsLevel);
        PlayerPrefs.SetInt("QualityLevel", graphicsLevel);
    }

    #region JsonFile
    private void CreateDefaulSession()
    {   
        SpaceShooterData defaultData = new SpaceShooterData
        {
            gameSpeed = Constants.DefaulGameSpeed,
            gameVolume = Constants.DefaulVolume,
            highScore = Constants.DefaulHighScore,
            shipSkinIndex = Constants.DefaulSkin,
            firstPlay = true,
            realisticMove = false,
            fireKey = KeyCode.Mouse0,
            limitLine = false,
            playerMovementOption = 0,
            joystick = false,
            graphicsIndex = _isWindows ? 5 : 2
        };

        string json = JsonConvert.SerializeObject(defaultData);
        File.WriteAllText(_sessionFilePath, json);
    }

    private static SpaceShooterData DeserializeObject()
    {
        string jsonData = File.ReadAllText(_sessionFilePath);
        return JsonConvert.DeserializeObject<SpaceShooterData>(jsonData);
    }

    private static void SerializeObject(SpaceShooterData jsonObject)
    {
        string json = JsonConvert.SerializeObject(jsonObject);
        File.WriteAllText(_sessionFilePath, json);
    }
    #endregion
    
    #region Volume
    public static int GetVolume()
    {
        var jsonObject = DeserializeObject();
        return jsonObject.gameVolume;
    }

    public static void SetVolume(int volume)
    {
        var jsonObject = DeserializeObject();
        
        if(volume != jsonObject.gameVolume)
        {
            jsonObject.gameVolume = volume;
            SerializeObject(jsonObject);
        }
    }

    public static void SetDefaultVolume()
    {
        var jsonObject = DeserializeObject();
        jsonObject.gameVolume = Constants.DefaulVolume;
        SerializeObject(jsonObject);
    }

    #endregion

    #region Skin
    public static int GetSkin()
    {
        var jsonObject = DeserializeObject();
        return jsonObject.shipSkinIndex;
    }

    public static void SetSkin(int skinIndex)
    {
        var jsonObject = DeserializeObject();
        
        if(skinIndex != jsonObject.shipSkinIndex)
        {
            jsonObject.shipSkinIndex = skinIndex;

            SerializeObject(jsonObject);
        }
    }

    public static void SetDefaultSkin()
    {
        var jsonObject = DeserializeObject();
        jsonObject.shipSkinIndex = Constants.DefaulSkin;
        SerializeObject(jsonObject);
    }

    #endregion

    #region GameSpeed
    public static float GetGameSpeed()
    {
        var jsonObject = DeserializeObject();
        return jsonObject.gameSpeed;
    }

    public static void SetGameSpeed(float gameSpeed)
    {
        var jsonObject = DeserializeObject();
        
        if(gameSpeed != jsonObject.gameSpeed)
        {
            jsonObject.gameSpeed = gameSpeed;

            SerializeObject(jsonObject);
        }
    }
    
    public static void SetDefaultGameSpeed()
    {
        var jsonObject = DeserializeObject();
        jsonObject.gameSpeed = Constants.DefaulGameSpeed;
        SerializeObject(jsonObject);
    }
    #endregion

    #region HighScore
    public static int GetHighScore()
    {
        var jsonObject = DeserializeObject();
        return jsonObject.highScore;
    }

    public static void SetHighScore(int highScore)
    {
        var jsonObject = DeserializeObject();

        if(highScore > jsonObject.highScore)
        {
            jsonObject.highScore = highScore;

            SerializeObject(jsonObject);
        }
    }
    
    public static void SetDefaultHighScore()
    {
        var jsonObject = DeserializeObject();
        jsonObject.highScore = Constants.DefaulHighScore;
        SerializeObject(jsonObject);
    }
    #endregion

    #region FirstPlay
    public static bool GetFirstPlay()
    {
        var jsonObject = DeserializeObject();
        return jsonObject.firstPlay;
    }

    public static void SetFirstPlay(bool firstPlay)
    {
        var jsonObject = DeserializeObject();

        if(firstPlay != jsonObject.firstPlay)
        {
            jsonObject.firstPlay = firstPlay;

            SerializeObject(jsonObject);
        }
    }
    #endregion

    #region RealisticBackground
     public static bool GetRealistic()
    {
        var jsonObject = DeserializeObject();
        return jsonObject.realisticMove;
    }

    public static void SetRealistic(bool realisticMove)
    {
        var jsonObject = DeserializeObject();

        if(realisticMove != jsonObject.realisticMove)
        {
            jsonObject.realisticMove = realisticMove;

            SerializeObject(jsonObject);
        }
    }
    
    public static void SetDefaultRealistic()
    {
        var jsonObject = DeserializeObject();
        jsonObject.realisticMove = false;
        SerializeObject(jsonObject);
    }
    
    #endregion

    #region LimiteLine
    public static bool GetLimiteLine()
    {
        var jsonObject = DeserializeObject();
        return jsonObject.limitLine;
    }

    public static void SetLimiteLine(bool limitLine)
    {
        var jsonObject = DeserializeObject();

        if(limitLine != jsonObject.limitLine)
        {
            jsonObject.limitLine = limitLine;

            SerializeObject(jsonObject);
        }
    }
    
    public static void SetDefaultLimitLine()
    {
        var jsonObject = DeserializeObject();
        jsonObject.limitLine = false;
        SerializeObject(jsonObject);
    }
    #endregion

    #region PlayerMovementControl
    public static int GetPlayerMovementControlOption()
    {
        var jsonObject = DeserializeObject();
        return jsonObject.playerMovementOption;
    }

    public static void SetPlayerMovementControlOption(int playerMovementOption)
    {
        var jsonObject = DeserializeObject();

        if(playerMovementOption != jsonObject.playerMovementOption)
        {
            jsonObject.playerMovementOption = playerMovementOption;

            SerializeObject(jsonObject);
        }
    }
    
    public static void SetDefaultPlayerMovementControlOption()
    {
        var jsonObject = DeserializeObject();
        jsonObject.playerMovementOption = 0;
        SerializeObject(jsonObject);
    }
    #endregion

    #region JoyStick
    public static bool GetJoystick()
    {
        var jsonObject = DeserializeObject();
        return jsonObject.joystick;
    }

    public static void SetJoystick(bool joystick)
    {
        var jsonObject = DeserializeObject();

        if(joystick != jsonObject.joystick)
        {
            jsonObject.joystick = joystick;

            SerializeObject(jsonObject);
        }
    }
    
    public static void SetDefaultJoystick()
    {
        var jsonObject = DeserializeObject();
        jsonObject.joystick = false;
        SerializeObject(jsonObject);
    }
    #endregion

    #region FireKey
    public static KeyCode GetFireKey()
    {
        var jsonObject = DeserializeObject();
        return jsonObject.fireKey;
    }

    public static void SetFireKey(KeyCode fireKey)
    {
        var jsonObject = DeserializeObject();

        if(fireKey != jsonObject.fireKey)
        {
            jsonObject.fireKey = fireKey;

            SerializeObject(jsonObject);
        }
    }
    
    public static void SetDefaultFireKey()
    {
        var jsonObject = DeserializeObject();
        jsonObject.fireKey = KeyCode.Mouse0;
        SerializeObject(jsonObject);
    }
    #endregion

    #region Graphics
    public static int GetGraphics()
    {
        var jsonObject = DeserializeObject();
        return jsonObject.graphicsIndex;
    }

    public static void SetGraphics(int graphicsIndex)
    {
        var jsonObject = DeserializeObject();

        if(graphicsIndex != jsonObject.graphicsIndex)
        {
            jsonObject.graphicsIndex = graphicsIndex;

            SerializeObject(jsonObject);
        }
    }
    
    public static void SetDefaultGraphics()
    {
        var jsonObject = DeserializeObject();

        if(GameIntroduction.IsWindows)
        {
            jsonObject.graphicsIndex = 5; //Ultra
        }
        else
        {
            jsonObject.graphicsIndex = 2; //Medium
        }

        SerializeObject(jsonObject);
    }
    #endregion

    [System.Serializable]
    private class SpaceShooterData
    {
        public float gameSpeed;
        public int gameVolume;
        public int highScore;
        public int shipSkinIndex;
        public bool firstPlay;
        public bool realisticMove;
        public bool limitLine;
        public int playerMovementOption;
        public bool joystick;
        public KeyCode fireKey;
        public int graphicsIndex;
    }
}
