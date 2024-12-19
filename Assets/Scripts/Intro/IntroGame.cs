using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class IntroGame : MonoBehaviour
{   
    public TextMeshProUGUI speedText;

    public static HashSet<KeyCode> invalidKeys;

    void Start()
    {
        StartCoroutine(TextAnimations.FadeTextIntro(speedText, 3f));

        invalidKeys = new HashSet<KeyCode>
        {
            KeyCode.Escape,
            KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4,
            KeyCode.F5, KeyCode.F6, KeyCode.F7, KeyCode.F8,
            KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12,
            KeyCode.Print, KeyCode.SysReq, KeyCode.ScrollLock,
            KeyCode.Pause, KeyCode.Break,
            KeyCode.LeftWindows, KeyCode.RightWindows,
            KeyCode.LeftCommand, KeyCode.RightCommand,
            KeyCode.Menu
        };
    }
}
