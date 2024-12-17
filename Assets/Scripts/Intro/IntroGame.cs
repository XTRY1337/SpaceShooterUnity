using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class IntroGame : MonoBehaviour
{   
    public TextMeshProUGUI speedText;
    public float fadeDuration;

    public static HashSet<KeyCode> invalidKeys;

    void Start()
    {
        speedText.alpha = 0f;
        StartCoroutine(FadeTextAndChangeScene());

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

    IEnumerator FadeTextAndChangeScene()
    {
        float elapsedTime = 0f;
        float phase1Duration = fadeDuration - 1f;
        float phase2Duration = 1f;

        while (elapsedTime < fadeDuration)
        {   
            elapsedTime += Time.deltaTime;

            if (elapsedTime <= phase1Duration)
            {
                float progress = elapsedTime / phase1Duration;
                speedText.alpha = Mathf.Lerp(0f, 0.3f, progress);
            }
            else
            {
                float progress = (elapsedTime - phase1Duration) / phase2Duration;
                speedText.alpha = Mathf.Lerp(0.3f, 1f, progress);
            }

            yield return null; //Wait next frame
        }

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(SessionManager.GetFirstPlay() ? 1 : 2);
    }
}
