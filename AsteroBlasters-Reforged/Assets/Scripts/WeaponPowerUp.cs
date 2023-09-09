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
        /// <summary>
        /// Prefab of projectile (or the raycast graphic representation, if weapon is raycast based) used by the given weapon
        /// </summary>
        [SerializeField] GameObject projectilePrefab;

        protected override void BuffPlayer(PlayerController playerController)
        {
            playerController.PickNewSecondaryWeapon(grantedWeapon, projectilePrefab);
            Destroy(gameObject);
        }
    }
}
