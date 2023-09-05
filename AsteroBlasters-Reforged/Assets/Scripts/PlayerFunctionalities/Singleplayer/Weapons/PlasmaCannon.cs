using UnityEngine;

namespace PlayerFunctionality
{
    /// <summary>
    /// Child class managing the functionalities of plasma cannon weapon
    /// </summary>
    public class PlasmaCannon : Weapon
    {
        [SerializeField] bool overheated;

        [SerializeField] float currentHeat;
        [SerializeField] float maxHeat;

        [SerializeField] float heatLoss;
        [SerializeField] float heatGain;

        public delegate void OnHeatChanged(float heat);
        public static OnHeatChanged onHeatChanged;

        public PlasmaCannon() 
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            fireCooldown = 0.1f;

            overheated = false;
            currentHeat = 0f;
            maxHeat = 1f;

            heatLoss = 0.005f;
            heatGain = 0.125f;
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

        public override GameObject Shoot()
        {
            // Firing the weapon if it's not overheated
            if (!overheated)
            {
                GameObject newPlasmaBurst = base.Shoot();

                // Increasing current heat and running the event
                currentHeat += heatGain;
                onHeatChanged?.Invoke(currentHeat);

                return newPlasmaBurst;
            }

            return null;
        }
    }
}
