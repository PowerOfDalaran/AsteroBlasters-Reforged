using PlayerFunctionality;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class managing the functionalities of plasma cannon weapon
    /// </summary>
    public class PlasmaCannon : Weapon
    {
        int maxAmmo;
        int currentAmmo;

        public delegate void OnAmmoValueChange(int current, int maximum);
        public event OnAmmoValueChange onAmmoValueChange;

        public override void InstantiateWeapon()
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            weaponClass = WeaponClass.PlasmaCannon;
            fireCooldown = 0.45f;

            maxAmmo = 20;
            currentAmmo = maxAmmo;
        }

        private void Start()
        {
            // Activating the event, so that the text box won't display "0/0" on start 
            onAmmoValueChange?.Invoke(currentAmmo, maxAmmo);
        }

        private void FixedUpdate()
        {
            //Checking if there's any ammo left, and discarding the weapon if not
            if (currentAmmo <= 0)
            {
                gameObject.GetComponent<PlayerController>().DiscardSecondaryWeapon();
            }
        }

        public override GameObject Shoot(float charge)
        {
            // Firing the weapon if ammunition didn't run out
            if (currentAmmo > 0)
            {
                GameObject newPlasmaBurst = base.Shoot(charge);

                // Checking if projectile was actually fired
                if (newPlasmaBurst != null)
                {
                    currentAmmo -= 1;
                    onAmmoValueChange?.Invoke(currentAmmo, maxAmmo);
                }

                return newPlasmaBurst;
            }

            return null;
        }
    }
}
