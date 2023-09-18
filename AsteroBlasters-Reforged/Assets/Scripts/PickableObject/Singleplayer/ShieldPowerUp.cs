using PlayerFunctionality;
using UnityEngine;

namespace PickableObjects
{
    /// <summary>
    /// Class managing the power ups granting shield to the person's picking it up
    /// </summary>
    public class ShieldPowerUp : PowerUp
    {
        [SerializeField] int grantedShield;

        protected override void BuffPlayer(PlayerController playerController)
        {
            // Giving shield to the person's picking it up by the grantedShield property, if it wouldn't cross the maxShield amount
            // Otherwise increasing their shield up to their maximum shield.
            playerController.currentShield += playerController.currentShield + grantedShield > playerController.maxShield ? playerController.maxShield : grantedShield;
        }
    }
}