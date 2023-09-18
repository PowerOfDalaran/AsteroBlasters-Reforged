using PlayerFunctionality;
using UnityEngine;

namespace PickableObjects
{
    /// <summary>
    /// Class managing the power ups healing the person's picking it up
    /// </summary>
    public class HealthPowerUp : PowerUp
    {
        [SerializeField] int amountOfHealing;

        protected override void BuffPlayer(PlayerController playerController)
        {
            // Healing the person's picking it up by the amountOfHealing property, if it wouldn't cross the maxHealth amount
            // Otherwise healing them up to their maximum health.
            playerController.currentHealth += playerController.currentHealth + amountOfHealing > playerController.maxHealth ? playerController.maxHealth : amountOfHealing;
        }
    }
}
