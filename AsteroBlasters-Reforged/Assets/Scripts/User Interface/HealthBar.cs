using PlayerFunctionality;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IRequirePlayerReference
{
    PlayerController playerController;
    NetworkPlayerController networkPlayerController;

    public void AddReferences(GameObject givenCharacter)
    {
        playerController = givenCharacter.GetComponent<PlayerController>();
        networkPlayerController = givenCharacter.GetComponent<NetworkPlayerController>();

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        // Subscribing to the events
        if (playerController != null)
        {
            playerController.onHealthChanged += UpdateTheHealthBar;
        }

        if (networkPlayerController != null)
        {
            networkPlayerController.onHealthChanged += UpdateTheHealthBar;
        }
    }

    void UpdateTheHealthBar(int currentHealth)
    {
        gameObject.GetComponent<Slider>().value = currentHealth;
    }
}
