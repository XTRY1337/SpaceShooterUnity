using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

public class TextAnimations
{
    public static IEnumerator FadeTextIntro(TextMeshProUGUI text, float fadeDuration)
    {
        yield return FadeText(text, fadeDuration);
               
        int sceneNumber = SessionManager.GetFirstPlay() ? 1 : 2;
        SceneManager.LoadScene(sceneNumber);
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

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
    }
}
