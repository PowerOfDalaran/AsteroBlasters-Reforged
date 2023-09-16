using PlayerFunctionality;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class managing the functionalities of the missile launcher weapon (network version) 
    /// </summary>
    public class NetworkMissileLauncher : NetworkWeapon
    {
        int maxAmmo;
        int currentAmmo;

        bool hadTarget;

        public delegate void OnAmmoValueChange(int current, int maximum);
        public event OnAmmoValueChange onAmmoValueChange;

        public delegate void OnTargetSwitch(Transform targetTransform);
        public event OnTargetSwitch onTargetSwitch;

        [SerializeField] GameObject targetedEnemy;
        [SerializeField] GameObject targetingZoneChild;
        [SerializeField] List<Collider2D> possibleTargets;

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

        private void OnEnable()
        {
            // Launching the event on start, so that the text box wouldn't start with "0/0" value
            onAmmoValueChange?.Invoke(currentAmmo, maxAmmo);

            //Creating new game object, attatching it to playerCharacter, adding the box collider to it and setting it up
            targetingZoneChild = new GameObject();

            // Setting up its initial world position, rotation and parent
            targetingZoneChild.transform.position = new Vector3(0, 0, 0);
            targetingZoneChild.transform.rotation = new Quaternion(0, 0, 0, 0);
            targetingZoneChild.transform.parent = transform;

            // Setting up its position and rotation in relation to its parent
            targetingZoneChild.name = "targetingZone";
            targetingZoneChild.transform.localPosition = new Vector3(0, 0, 0);
            targetingZoneChild.transform.localRotation = new Quaternion(0, 0, 0, 0);

            // Changing the layer to the "IgnoreProjectile", so that the projectiles etc. won't destroy themselfs
            targetingZoneChild.layer = 8;

            // Adding the box collider and positioning it 
            BoxCollider2D targetingZone = targetingZoneChild.AddComponent<BoxCollider2D>();
            targetingZone.isTrigger = true;
            targetingZone.offset = new Vector2(0, 2);
            targetingZone.size = new Vector2(9, 3);
        }

        private void OnDisable()
        {
            // Destroying the created child, if weapon would be uneqipped
            Destroy(targetingZoneChild);
        }

        private void FixedUpdate()
        {
            //Checking if there's any ammo left, and discarding the weapon if not
            if (currentAmmo <= 0)
            {
                gameObject.GetComponent<NetworkPlayerController>().DiscardSecondaryWeapon();
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
            NetworkPlayerController playerController = collision.gameObject.GetComponent<NetworkPlayerController>();

            if (playerController != null)
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
        /// Method finding new target within the box collider and assigning it to "targeted enemy" variable
        /// </summary>
        void FindNewTargetInRange()
        {
            if (possibleTargets.Count > 0)
            {
                targetedEnemy = possibleTargets[0].gameObject;
                onTargetSwitch?.Invoke(targetedEnemy.transform);
            }
        }

        protected override void AccessCreatedProjectile(GameObject projectile)
        {
            if (targetedEnemy == null)
            {
                projectile.GetComponent<NetworkHomingMissile>().Target = null;
            }
            else
            {
                projectile.GetComponent<NetworkHomingMissile>().Target = targetedEnemy.transform;
            }
        }

        public override bool Shoot(float charge)
        {
            // Checking if player can shoot
            if (currentAmmo > 0)
            {
                bool weaponFired = base.Shoot(charge);

                // If the missile was created and the player has target, it is assigned to the homing missile
                if (weaponFired)
                {
                    currentAmmo -= 1;
                    onAmmoValueChange?.Invoke(currentAmmo, maxAmmo);
                    return true;
                }
                return false;

            }
            return false;
        }
    }

}