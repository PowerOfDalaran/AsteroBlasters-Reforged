using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerFunctionality
{
    public class MissileLauncher : Weapon
    {
        int maxAmmo;
        int currentAmmo;

        [SerializeField] BoxCollider2D targetingZone;

        public delegate void OnTargetSwitch(Transform targetTransform);
        public static OnTargetSwitch onTargetSwitch;

        public GameObject targetedEnemy;
        public bool hadTarget;

        public MissileLauncher()
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            fireCooldown = 2f;

            maxAmmo = 3;
            currentAmmo = maxAmmo;
            hadTarget = false;
        }

        private void FixedUpdate()
        {
            // Checking if there's any ammo left
            //if (currentAmmo <= 0) 
            //{
            //    PlasmaCannon plasmaCannon = GetComponent<PlasmaCannon>();
            //    DiscardWeapon(plasmaCannon);
            //}

            if (targetedEnemy == null && hadTarget)
            {
                onTargetSwitch?.Invoke(null);
                hadTarget = false;
                FindNewTargetInRange();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IHealthSystem healthSystem = collision.gameObject.GetComponent<IHealthSystem>();

            if (targetedEnemy == null && healthSystem != null)
            {
                hadTarget = true;
                targetedEnemy = collision.gameObject;
                onTargetSwitch?.Invoke(targetedEnemy.transform);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (targetedEnemy == collision.gameObject)
            {
                hadTarget = true;
                targetedEnemy = null; 
                onTargetSwitch?.Invoke(null);
            }
        }

        void FindNewTargetInRange()
        {
            List<Collider2D> possibleTargets = new List<Collider2D>();
            ContactFilter2D contactFilter = new ContactFilter2D();

            GetComponent<BoxCollider2D>().OverlapCollider(contactFilter, possibleTargets);
            if (possibleTargets.Count > 0)
            {
                targetedEnemy = possibleTargets[0].gameObject;
                onTargetSwitch?.Invoke(targetedEnemy.transform);
            }
        }

        public override GameObject Shoot()
        {
            // Checking if player can shoot
            if (currentAmmo > 0)
            {
                GameObject newMissile =  base.Shoot();
                if (newMissile != null)
                {
                    if (targetedEnemy == null)
                    {
                        newMissile.GetComponent<HomingMissile>().target = null;
                    }
                    else
                    {
                        newMissile.GetComponent<HomingMissile>().target = targetedEnemy.transform;
                    }
                    currentAmmo -= 1;

                    return newMissile;
                }
            }
            return null;
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
