using PlayerFunctionality;
using UnityEngine;
using WeaponSystem;

namespace PickableObjects
{
    /// <summary>
    /// Class managing the power ups granting the new weapon for the player
    /// </summary>
    public class NetworkWeaponPowerUp : NetworkPowerUp
    {
        [SerializeField] public WeaponClass grantedWeapon;

        protected override void BuffPlayer(NetworkPlayerController playerController)
        {
            playerController.PickNewSecondaryWeapon(grantedWeapon);
        }
    }
}
