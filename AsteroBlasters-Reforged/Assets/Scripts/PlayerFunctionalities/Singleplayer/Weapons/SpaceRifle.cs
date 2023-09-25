using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class providing the functionality for base player weapon.
    /// </summary>
    public class SpaceRifle : Weapon
    {
        bool overheated;

        float currentHeat;
        float maxHeat;

        float heatLoss;
        float heatGain;

        public delegate void OnHeatChanged(float heat);
        public event OnHeatChanged onHeatChanged;

        private void Start()
        {
            // Instantiating the wepon parameters on start, since this weapon is always active and available for player
            InstantiateWeapon();
        }

        private void FixedUpdate()
        {
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

        public override void InstantiateWeapon()
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            weaponClass = WeaponClass.SpaceRifle;
            fireCooldown = 0.125f;

            overheated = false;
            currentHeat = 0f;
            maxHeat = 1f;

            heatLoss = 0.0085f;
            heatGain = 0.15f;
        }

        public override GameObject Shoot(float charge)
        {
            // Firing the weapon if it's not overheated 
            if (!overheated)
            {
                GameObject newSpaceBullet = base.Shoot(charge);

                // Checking if bullet was actually fired
                if (newSpaceBullet != null)
                {
                    // Increasing current heat and running the event
                    currentHeat += heatGain;
                    onHeatChanged?.Invoke(currentHeat);
                }

                return newSpaceBullet;
            }

            return null;
        }
    }
}
