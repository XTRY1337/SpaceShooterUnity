using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class GameIntroduction : MonoBehaviour
{   
    [SerializeField] private TextMeshProUGUI _introductionText;

    private static HashSet<KeyCode> _invalidRemappingKeys;
    private static HashSet<KeyCode> _tutorialAvailableKeys;

    public static HashSet<KeyCode> InvalidRemappingKeys => _invalidRemappingKeys;
    public static HashSet<KeyCode> TutorialAvailableKeys => _tutorialAvailableKeys;

    void Start()
    {
        StartCoroutine(TextAnimations.FadeTextIntro(_introductionText, 3f));

        _invalidRemappingKeys = new HashSet<KeyCode>
        {
            KeyCode.Escape, KeyCode.Menu, KeyCode.Pause, KeyCode.Break,
            KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4,
            KeyCode.F5, KeyCode.F6, KeyCode.F7, KeyCode.F8,
            KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12,
            KeyCode.Print, KeyCode.SysReq, KeyCode.ScrollLock,
            KeyCode.LeftWindows, KeyCode.RightWindows, 
            KeyCode.LeftCommand, KeyCode.RightCommand
        };

        _tutorialAvailableKeys = new HashSet<KeyCode>
        {
            KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D,
            KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow
        };
    }
}
