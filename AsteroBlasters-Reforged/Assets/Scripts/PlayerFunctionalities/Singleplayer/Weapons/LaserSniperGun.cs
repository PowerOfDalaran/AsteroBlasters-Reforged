using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            type = WeaponType.RaycastBased;
            fireCooldown = 1.5f;

            maxAmmo = 10;
            currentAmmo = maxAmmo;
        }

        #region Adding and removing raycast laser

        private void Start()
        {
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
            if (currentAmmo > 0 && Time.time > cooldownStatus)
            {
                GameObject hitTarget = base.Shoot(charge);
                StartCoroutine(DrawRaycastLaser(hitTarget));

                // Deal damage based on charged energy
                if (hitTarget != null)
                {
                    IHealthSystem healthSystem = hitTarget.GetComponent<IHealthSystem>();

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

            return null;
        }

        IEnumerator DrawRaycastLaser(GameObject hitTarget)
        {
            raycastLaser.enabled = true;

            if (hitTarget != null)
            {
                raycastLaser.SetPosition(0, firePoint.position);
                raycastLaser.SetPosition(1, firePoint.position + firePoint.up * Vector2.Distance(firePoint.position, hitTarget.transform.position));
            }
            else
            {
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
