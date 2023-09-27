using PlayerFunctionality;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject playerController;
    [SerializeField] GameObject networkPlayerController;

    private void Start()
    {
        // Subscribing to the events
        if (playerController != null)
        {
            playerController.GetComponent<PlayerController>().onHealthChanged += UpdateTheHealthBar;
        }

        if (networkPlayerController != null)
        {
            networkPlayerController.GetComponent<NetworkPlayerController>().onHealthChanged += UpdateTheHealthBar;
        }
    }

    void UpdateTheHealthBar(int currentHealth)
    {
        gameObject.GetComponent<Slider>().value = currentHealth;
    }
}
