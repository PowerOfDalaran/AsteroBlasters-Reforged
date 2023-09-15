using PlayerFunctionality;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class managing the functionalities of secondary weapon - plasma cannon (network version)
    /// </summary>
    public class NetworkPlasmaCannon : NetworkWeapon
    {
        [SerializeField] bool overheated;

        [SerializeField] float currentHeat;
        [SerializeField] float maxHeat;

        float heatLoss;
        float heatGain;

        [SerializeField] int maxAmmo;
        [SerializeField] int currentAmmo;

        public delegate void OnAmmoValueChange(int current, int maximum);
        public event OnAmmoValueChange onAmmoValueChange;

        public delegate void OnHeatChanged(float heat);
        public event OnHeatChanged onHeatChanged;

        public override void InstantiateWeapon()
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            weaponClass = WeaponClass.PlasmaCannon;
            fireCooldown = 0.25f;

            overheated = false;
            currentHeat = 0f;
            maxHeat = 1f;

            heatLoss = 0.0075f;
            heatGain = 0.3f;

            maxAmmo = 14;
            currentAmmo = maxAmmo;
        }

        private void Start()
        {
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

            // Checking wether the weapon is overheated, or should be overheated, or isn't overheated
            if (overheated)
            {
                // Decreasing current heat and running the event
                currentHeat -= heatLoss;
                onHeatChanged?.Invoke(currentHeat);

                // Checking if weapon should still be overheated 
                if (currentHeat <= 0f)
                {
                    overheated = false;
                }
            }
            else if (currentHeat >= maxHeat)
            {
                // Turning overheated state to true
                overheated = true;
            }
            else if (currentHeat > 0)
            {
                // Decreasing current heat and running the event
                currentHeat -= heatLoss;
                onHeatChanged?.Invoke(currentHeat);
            }
        }

        public override bool Shoot(float charge)
        {
            if (!overheated && currentAmmo > 0)
            {
                bool weaponFired = base.Shoot(charge);

                if (weaponFired)
                {
                    // If weapon actually fired, removing ammunition, generating heat and launching the events
                    currentAmmo -= 1;
                    currentHeat += heatGain;

                    onHeatChanged?.Invoke(currentHeat);
                    onAmmoValueChange?.Invoke(currentAmmo, maxAmmo);

                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
