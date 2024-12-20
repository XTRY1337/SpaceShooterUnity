using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{   
    private void Start()
    {
        BackgroundMovement.MovementOption = 0;
        AudioManager.Instance.SetNewMusic(0);
    }

    public void OnPlayButton()
    {
        AudioManager.Instance.SetNewMusic(1);
        SceneManager.LoadScene(4);
    }

    public void OnSettingsButton()
    {
        SceneManager.LoadScene(3);
    }

    public void OnHowToPlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
