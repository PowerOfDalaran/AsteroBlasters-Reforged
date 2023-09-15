using PlayerFunctionality;
using UnityEngine;
using WeaponSystem;

namespace PickableObjects
{
    /// <summary>
    /// Class managing the power ups granting the new weapon to person's picking it up
    /// </summary>
    public class WeaponPowerUp : PowerUp
    {
        [SerializeField] WeaponClass grantedWeapon;


        protected override void BuffPlayer(PlayerController playerController)
        {
            playerController.PickNewSecondaryWeapon(grantedWeapon);
            Destroy(gameObject);
        }
    }
}
