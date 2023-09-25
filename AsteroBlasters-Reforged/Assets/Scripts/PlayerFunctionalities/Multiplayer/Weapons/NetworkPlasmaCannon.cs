using PlayerFunctionality;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class managing the functionalities of secondary weapon - plasma cannon (network version)
    /// </summary>
    public class NetworkPlasmaCannon : NetworkWeapon
    {
        [SerializeField] int maxAmmo;
        [SerializeField] int currentAmmo;

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

        private void OnEnable()
        {
            // Launching the event on start, so that the text box wouldn't start with "0/0" value
            onAmmoValueChange?.Invoke(currentAmmo, maxAmmo);
        }

        private void FixedUpdate()
        {
            // Deciding whether the rest of method should be activated
            if (!IsOwner)
            {
                return;
            }

            //Checking if there's any ammo left, and discarding the weapon if not
            if (currentAmmo <= 0)
            {
                gameObject.GetComponent<NetworkPlayerController>().DiscardSecondaryWeapon();
            }
        }

        public override bool Shoot(float charge)
        {
            if (currentAmmo > 0)
            {
                bool weaponFired = base.Shoot(charge);

                if (weaponFired)
                {
                    // If weapon actually fired, removing ammunition and launching the events
                    currentAmmo -= 1;
                    onAmmoValueChange?.Invoke(currentAmmo, maxAmmo);

                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
