using PlayerFunctionality;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class managing the functionalities of the missile launcher weapon
    /// </summary>
    public class MissileLauncher : Weapon
    {
        int maxAmmo;
        int currentAmmo;

        public delegate void OnTargetSwitch(Transform targetTransform);
        public event OnTargetSwitch onTargetSwitch;

        public delegate void OnAmmoValueChange(int current, int maximum);
        public event OnAmmoValueChange onAmmoValueChange;

        List<Collider2D> possibleTargets = new List<Collider2D>();
        GameObject targetingZoneChild;
        GameObject targetedEnemy;
        bool hadTarget;

        public override void InstantiateWeapon()
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            weaponClass = WeaponClass.MissileLauncher;
            fireCooldown = 2f;


            maxAmmo = 3;
            currentAmmo = maxAmmo;
            hadTarget = false;
        }

        #region Adding and removing targeting zone (mostly)

        private void Start()
        {
            // Activating the event
            onAmmoValueChange?.Invoke(currentAmmo, maxAmmo);

            //Creating new game object, attatching it to playerCharacter, adding the box collider to it and setting it up
            targetingZoneChild = Instantiate(new GameObject(), new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), transform);

            targetingZoneChild.name = "targetingZone";
            targetingZoneChild.transform.localPosition = new Vector3(0, 0, 0);
            targetingZoneChild.layer = 8;

            BoxCollider2D targetingZone = targetingZoneChild.AddComponent<BoxCollider2D>(); 
            targetingZone.isTrigger = true;
            targetingZone.offset = new Vector2(0, 2);
            targetingZone.size = new Vector2(9, 3);
        }

        private void OnDestroy()
        {
            Destroy(targetingZoneChild);
        }
        #endregion

        private void FixedUpdate()
        {
            //Checking if there's any ammo left, and discarding the weapon if not
            if (currentAmmo <= 0)
            {
                gameObject.GetComponent<PlayerController>().DiscardSecondaryWeapon();
            }

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
            // Checking if leaving object was on targets list, if so - removing it
            if (possibleTargets.Contains(collision))
            {
                possibleTargets.Remove(collision);
            }

            // Checking if leaving object was current target
            if (targetedEnemy == collision.gameObject)
            {
                hadTarget = true;
                targetedEnemy = null; 
                onTargetSwitch?.Invoke(null);
            }
        }

        /// <summary>
        /// Method looking for another target, which has already entered the targeting zone
        /// </summary>
        void FindNewTargetInRange()
        {
            if (possibleTargets.Count > 0)
            {
                targetedEnemy = possibleTargets[0].gameObject;
                onTargetSwitch?.Invoke(targetedEnemy.transform);
            }
        }

        public override GameObject Shoot(float charge)
        {
            // Checking if player can shoot
            if (currentAmmo > 0)
            {
                GameObject newMissile =  base.Shoot(charge);

                // If the missile was created and the player has target, it is assigned to the homing missile
                if (newMissile != null)
                {
                    if (targetedEnemy == null)
                    {
                        newMissile.GetComponent<HomingMissile>().Target = null;
                    }
                    else
                    {
                        newMissile.GetComponent<HomingMissile>().Target = targetedEnemy.transform;
                    }

                    currentAmmo -= 1;
                    onAmmoValueChange?.Invoke(currentAmmo, maxAmmo);

                    return newMissile;
                }
            }
            return null;
        }
    }
}
