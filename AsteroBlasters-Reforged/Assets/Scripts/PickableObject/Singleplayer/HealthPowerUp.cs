using PlayerFunctionality;
using UnityEngine;

namespace PickableObjects
{
    public class HealthPowerUp : PowerUp
    {
        [SerializeField] int amountOfHealing;

        protected override void BuffPlayer(PlayerController playerController)
        {
            Debug.Log(playerController.currentHealth);
            playerController.currentHealth += playerController.currentHealth + amountOfHealing > playerController.maxHealth ? playerController.maxHealth : amountOfHealing;
            Debug.Log(playerController.currentHealth);
        }
    }
}
