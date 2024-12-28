using UnityEngine;

public class UpgradeContact : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {   
        if(other.tag == "Boundary" || other.tag == "Asteroid")
        {   
            return;
        }

        if(other.tag == "Player")
        {   
            //TODO: add if condition if add different upgrades
            PlayerController.Instance.ChangeShieldState(true);
            Destroy(gameObject);
        }
    }
}
