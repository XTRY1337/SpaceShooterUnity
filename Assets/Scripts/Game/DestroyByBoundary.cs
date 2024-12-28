using UnityEngine;

public class DestroyByBoundary : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {   
        if(other.gameObject.tag == "Asteroid" && !TutorialManager.IsTutorial)
        {   
            DestroyByContact.ResetAsteroidSequence();
            GameManager.Instance.ResetMultiplierText();
        }

        Destroy(other.gameObject);
    }
}
