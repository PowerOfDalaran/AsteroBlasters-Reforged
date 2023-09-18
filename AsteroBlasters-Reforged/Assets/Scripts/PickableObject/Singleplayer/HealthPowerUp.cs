using PlayerFunctionality;
using UnityEngine;

namespace PickableObjects
{
    public class HealthPowerUp : PowerUp
    {
        [SerializeField] int amountOfHealing;

        protected override void BuffPlayer(PlayerController playerController)
        {
            playerController.currentHealth += playerController.currentHealth + amountOfHealing > playerController.maxHealth ? playerController.maxHealth : amountOfHealing;
        }
    }
}
