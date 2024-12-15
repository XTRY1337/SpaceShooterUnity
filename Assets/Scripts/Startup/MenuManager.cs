using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{   
    private void Start()
    {
        BackgroundMovement.movementOption = 0;
    }

    public void OnPlayButton()
    {
        AudioManager.instance.ChangeMusic(1);
        SceneManager.LoadScene(4);
    }

    public void OnSettingsButton()
    {
        SceneManager.LoadScene(3);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    public void OnBackMenuButton()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        AudioManager.instance.ChangeMusic(0);
        SceneManager.LoadScene(2);
    }

    public void OnHowToPlayButton()
    {
        SceneManager.LoadScene(1);
    }
}
