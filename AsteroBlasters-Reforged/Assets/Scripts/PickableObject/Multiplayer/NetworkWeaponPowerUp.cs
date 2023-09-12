using PlayerFunctionality;
using UnityEngine;
using WeaponSystem;

namespace PickableObjects
{
    public class NetworkWeaponPowerUp : NetworkPowerUp
    {
        [SerializeField] WeaponClass grantedWeapon;
        [SerializeField] GameObject projectilePrefab;

        protected override void BuffPlayer(NetworkPlayerController playerController)
        {
            playerController.PickNewSecondaryWeapon(grantedWeapon, projectilePrefab);
        }
    }
}
