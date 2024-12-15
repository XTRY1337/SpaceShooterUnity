using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroGame : MonoBehaviour
{   
    public TextMeshProUGUI speedText;
    public float fadeDuration;

    void Start()
    {
        speedText.alpha = 0f;
        StartCoroutine(FadeTextAndChangeScene());
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
