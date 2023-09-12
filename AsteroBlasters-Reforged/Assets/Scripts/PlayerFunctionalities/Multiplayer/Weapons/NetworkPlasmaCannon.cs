using PlayerFunctionality;
using UnityEngine;

namespace WeaponSystem
{
    public class NetworkPlasmaCannon : NetworkWeapon
    {
        [SerializeField] bool overheated;

        [SerializeField] float currentHeat;
        [SerializeField] float maxHeat;

        float heatLoss;
        float heatGain;

        [SerializeField] int maxAmmo;
        [SerializeField] int currentAmmo;

        private void Start()
        {
            InstantiateWeapon(projectilePrefab);
        }

        public override void InstantiateWeapon(GameObject projectile)
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            weaponClass = WeaponClass.PlasmaCannon;
            fireCooldown = 0.25f;
            projectilePrefab = projectile;

            overheated = false;
            currentHeat = 0f;
            maxHeat = 1f;

            heatLoss = 0.0085f;
            heatGain = 0.2f;

            maxAmmo = 14;
            currentAmmo = maxAmmo;
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
            }
        }

        public override void Shoot(float charge)
        {
            if (!overheated && currentAmmo > 0)
            {
                currentAmmo -= 1;

                base.Shoot(charge);

                // Increasing current heat and running the event
                currentHeat += heatGain;

            }
        }
    }
}
