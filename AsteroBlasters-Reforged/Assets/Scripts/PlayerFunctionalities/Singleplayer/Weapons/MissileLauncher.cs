using System.Collections.Generic;
using UnityEngine;

namespace PlayerFunctionality
{
    /// <summary>
    /// Class managing the functionalities of the missile launcher weapon
    /// </summary>
    public class MissileLauncher : Weapon
    {
        int maxAmmo;
        int currentAmmo;

        public delegate void OnTargetSwitch(Transform targetTransform);
        public static OnTargetSwitch onTargetSwitch;


        public List<Collider2D> possibleTargets = new List<Collider2D>();
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
        #region Adding and removing targeting zone
        private void Start()
        {
            BoxCollider2D targetingZone = gameObject.AddComponent<BoxCollider2D>();

            targetingZone.isTrigger = true;
            targetingZone.offset = new Vector2(0, 2);
            targetingZone.size = new Vector2(9, 3);
        }

        private void OnDestroy()
        {
            Destroy(gameObject.GetComponent<BoxCollider2D>());
        }
        #endregion

        private void FixedUpdate()
        {
            // Checking if there's any ammo left
            //if (currentAmmo <= 0) 
            //{
            //    PlasmaCannon plasmaCannon = GetComponent<PlasmaCannon>();
            //    DiscardWeapon(plasmaCannon);
            //}

            // Checking if the current target was removed from targeting zone without leaving it
            if (targetedEnemy == null && hadTarget)
            {
                onTargetSwitch?.Invoke(null);
                hadTarget = false;
                FindNewTargetInRange();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Checking if detected collider is an proper target and adding it to targets list
            IHealthSystem healthSystem = collision.gameObject.GetComponent<IHealthSystem>();

            if (healthSystem != null)
            {
                possibleTargets.Add(collision);

                if (targetedEnemy == null)
                {
                    hadTarget = true;
                    targetedEnemy = collision.gameObject;
                    onTargetSwitch?.Invoke(targetedEnemy.transform);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {

            if (possibleTargets.Contains(collision))
            {
                possibleTargets.Remove(collision);
            }

            if (targetedEnemy == collision.gameObject)
            {
                hadTarget = true;
                targetedEnemy = null; 
                onTargetSwitch?.Invoke(null);
            }
        }

        void FindNewTargetInRange()
        {
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
