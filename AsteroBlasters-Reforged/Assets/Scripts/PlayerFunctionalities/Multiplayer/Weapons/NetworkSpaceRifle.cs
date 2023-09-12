using UnityEngine;

namespace WeaponSystem
{
    public class NetworkSpaceRifle : NetworkWeapon
    {
        void Start()
        {
            InstantiateWeapon();
        }

        public override void InstantiateWeapon(GameObject projectile = null)
        {
            type = WeaponType.ProjectileBased;
            weaponClass = WeaponClass.SpaceRifle;
            fireCooldown = 0.35f;
        }
    }
}
