using System.Collections;
using UnityEngine;

namespace PlayerFunctionality
{
    public class LaserSniperGun : Weapon
    {
        int maxAmmo;
        int currentAmmo;

        [SerializeField] GameObject raycastLaserPrefab;
        LineRenderer raycastLaser;

        public LaserSniperGun()
        {
            // Assigning the values to the properties
            type = WeaponType.RaycastBased;
            fireCooldown = 1.5f;

            maxAmmo = 10;
            currentAmmo = maxAmmo;
        }

        #region Adding and removing raycast laser

        private void Start()
        {
            // Setting up the line renderer prefab
            GameObject createdRaycastLaser = Instantiate(raycastLaserPrefab);
            createdRaycastLaser.transform.parent = transform;

            createdRaycastLaser.transform.position = Vector2.zero;
            raycastLaser = createdRaycastLaser.GetComponent<LineRenderer>();

            raycastLaser.enabled = false;
        }

        private void OnDestroy()
        {
            Destroy(gameObject.transform.Find("RaycastLaser"));
        }
        #endregion

        private void FixedUpdate()
        {
            // Checking if there's any ammo left, and discarding the weapon if not
            //if (currentAmmo <= 0) 
            //{
            //    PlasmaCannon plasmaCannon = GetComponent<PlasmaCannon>();
            //    DiscardWeapon(plasmaCannon);
            //}
        }

        public override GameObject Shoot(float charge)
        {
            // Checking if the shot can be taken
            if (currentAmmo > 0 && Time.time > cooldownStatus)
            {
                // Activating the raycast in the parent class and displaying the laser on screen
                GameObject hitTarget = base.Shoot(charge);
                StartCoroutine(DrawRaycastLaser(hitTarget));

                if (hitTarget != null)
                {
                    // Checking if hit target is a proper enemy
                    IHealthSystem healthSystem = hitTarget.GetComponent<IHealthSystem>();

                    if (healthSystem != null)
                    {
                        // Deal damage based on charged energy
                        if (charge <= 6)
                        {
                            healthSystem.TakeDamage(1);
                        }
                        else if (charge < 10)
                        {
                            healthSystem.TakeDamage(2);
                        }
                        else
                        {
                            healthSystem.TakeDamage(3);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// IEnumerator drawing showing and placing correctly the line of laser to display the fired shot
        /// </summary>
        /// <param name="hitTarget">Game object of hit target</param>
        /// <returns></returns>
        IEnumerator DrawRaycastLaser(GameObject hitTarget)
        {
            raycastLaser.enabled = true;

            if (hitTarget != null)
            {
                // Setting the laser to reach the hit enemy
                raycastLaser.SetPosition(0, firePoint.position);
                raycastLaser.SetPosition(1, firePoint.position + firePoint.up * raycastDistance);
            }
            else
            {
                // Setting the laser to reach some distance before vanishing
                raycastLaser.SetPosition(0, firePoint.position);
                raycastLaser.SetPosition(1, firePoint.position + firePoint.up * 100);
            }

            yield return new WaitForSeconds(0.12f);

            raycastLaser.enabled = false;
        }

        /// <summary>
        /// Method activating removing this script and activating given weapon 
        /// </summary>
        /// <param name="weaponToActivate">Script of weapon, player switches to</param>
        public void DiscardWeapon(Weapon weaponToActivate)
        {
            weaponToActivate.enabled = true;

            Destroy(this);
        }
    }
}
