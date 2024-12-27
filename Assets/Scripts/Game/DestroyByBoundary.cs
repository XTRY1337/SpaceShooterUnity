using UnityEngine;

public class DestroyByBoundary : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {   
        if(other.gameObject.tag == "Asteroid" && !TutorialManager.IsTutorial)
        {
            GameManager.Instance.ResetMultiplierText();
        }

        Destroy(other.gameObject);
    }
}
