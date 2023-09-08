using PlayerFunctionality;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Child class managing the functionalities of plasma cannon weapon
    /// </summary>
    public class PlasmaCannon : Weapon
    {
        bool overheated;

        float currentHeat;
        float maxHeat;

        float heatLoss;
        float heatGain;

        int maxAmmo;
        int currentAmmo;

        public delegate void OnHeatChanged(float heat);
        public static event OnHeatChanged onHeatChanged;

        public PlasmaCannon() 
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            weaponClass = WeaponClass.PlasmaCannon;
            fireCooldown = 0.25f;

            overheated = false;
            currentHeat = 0f;
            maxHeat = 1f;

            heatLoss = 0.0085f;
            heatGain = 0.2f;

            maxAmmo = 15;
            currentAmmo = maxAmmo;
        }

        private void FixedUpdate()
        {
            //Checking if there's any ammo left, and discarding the weapon if not
            if (currentAmmo <= 0)
            {
                gameObject.GetComponent<PlayerController>().DiscardSecondaryWeapon();
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

        public override GameObject Shoot(float charge)
        {
            // Firing the weapon if it's not overheated
            if (!overheated)
            {
                GameObject newPlasmaBurst = base.Shoot(charge);

                // Increasing current heat and running the event
                currentHeat += heatGain;
                onHeatChanged?.Invoke(currentHeat);

                return newPlasmaBurst;
            }

            return null;
        }
    }
}
