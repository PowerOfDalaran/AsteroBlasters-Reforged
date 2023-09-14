using UnityEngine;

namespace WeaponSystem
{
    public class SpaceRifle : Weapon
    {
        private void Start()
        {
            InstantiateWeapon();
        }

        public override void InstantiateWeapon()
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            weaponClass = WeaponClass.SpaceRifle;
            fireCooldown = 0.35f;
        }
    }
}
