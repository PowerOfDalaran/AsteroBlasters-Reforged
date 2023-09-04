using UnityEngine;

namespace PlayerFunctionality
{
    /// <summary>
    /// Child class managing the functionalities of plasma cannon weapon
    /// </summary>
    public class PlasmaCannon : Weapon
    {
        [SerializeField]  bool overheated;

        [SerializeField] float currentHeat;
        [SerializeField] float maxHeat;

        public PlasmaCannon() 
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            fireCooldown = 0.1f;

            overheated = false;
            currentHeat = 0f;
            maxHeat = 1f;
        }

        private void FixedUpdate()
        {
            // Checking wether the weapon is overheated, or should be overheated, or isn't overheated
            if (overheated)
            {
                // Decreasing current heat
                currentHeat -= 0.005f;

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
                // Decreasing current heat
                currentHeat -= 0.005f;
            }
        }

        public override void Shoot()
        {
            // Firing the weapon if it's not overheated
            if (!overheated)
            {
                base.Shoot();
                currentHeat += 0.125f;
            }
        }
    }
}
