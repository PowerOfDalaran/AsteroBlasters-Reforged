using PlayerFunctionality;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] NetworkPlayerController networkPlayerController;

    private void Start()
    {
        // Subscribing to the events
        if (playerController != null)
        {
            playerController.GetComponent<PlayerController>().on
        }
    }
}
