using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    public GameObject closeButton;

    void Start()
    {
        closeButton.SetActive(false);

        if(!SessionManager.GetFirstPlay())
        {
            closeButton.SetActive(true);
        }

        StartCoroutine(StartTutorial());
    }

    void Update()
    {
        
    }

    IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(5);

        SessionManager.SetFirstPlay(false);
        SceneManager.LoadScene(2);
    }

    public void OnCloseButton()
    {
        StopCoroutine(StartTutorial());
        SceneManager.LoadScene(2);
    }
}
