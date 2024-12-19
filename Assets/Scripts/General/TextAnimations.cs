using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextAnimations
{
    public static IEnumerator FadeTextIntro(TextMeshProUGUI text, float fadeDuration)
    {
        yield return FadeText(text, fadeDuration);
               
        SceneManager.LoadScene(SessionManager.GetFirstPlay() ? 1 : 2);
    }

    public static IEnumerator FadeText(TextMeshProUGUI text, float fadeDuration)
    {
        float elapsedTime = 0f;
        float phase1Duration = fadeDuration - 1f;
        float phase2Duration = 1f;

        text.alpha = 0;

        while (elapsedTime < fadeDuration)
        {   
            elapsedTime += Time.deltaTime;  

            if (elapsedTime <= phase1Duration)
            {
                float progress = elapsedTime / phase1Duration;
                text.alpha = Mathf.Lerp(0f, 0.3f, progress);
            }
            else
            {
                float progress = (elapsedTime - phase1Duration) / phase2Duration;
                text.alpha = Mathf.Lerp(0.3f, 1f, progress);
            }

            yield return null; //Wait next frame
        }

        yield return new WaitForSeconds(0.5f);
    }
}
