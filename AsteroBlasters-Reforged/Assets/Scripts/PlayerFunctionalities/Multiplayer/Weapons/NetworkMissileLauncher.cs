using PlayerFunctionality;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WeaponSystem.MissileLauncher;

namespace WeaponSystem
{
    public class NetworkMissileLauncher : NetworkWeapon
    {
        int maxAmmo;
        int currentAmmo;

        bool hadTarget;
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
            //Creating new game object, attatching it to playerCharacter, adding the box collider to it and setting it up
            targetingZoneChild = new GameObject();

            targetingZoneChild.transform.position = new Vector3(0, 0, 0);
            targetingZoneChild.transform.rotation = new Quaternion(0, 0, 0, 0);
            targetingZoneChild.transform.parent = transform;

            targetingZoneChild.name = "targetingZone";
            targetingZoneChild.transform.localPosition = new Vector3(0, 0, 0);
            targetingZoneChild.transform.localRotation = new Quaternion(0, 0, 0, 0);
            targetingZoneChild.layer = 8;

            BoxCollider2D targetingZone = targetingZoneChild.AddComponent<BoxCollider2D>();
            targetingZone.isTrigger = true;
            targetingZone.offset = new Vector2(0, 2);
            targetingZone.size = new Vector2(9, 3);
        }

        private void OnDisable()
        {
            Destroy(targetingZoneChild);
        }

        private void FixedUpdate()
        {
            //Checking if there's any ammo left, and discarding the weapon if not
            if (currentAmmo <= 0)
            {
                gameObject.GetComponent<NetworkPlayerController>().DiscardSecondaryWeaponLocally();
                gameObject.GetComponent<NetworkPlayerController>().DiscardSecondaryWeaponOnHostServerRpc();

            }

            // Checking if the current target was removed from targeting zone without leaving it
            if (targetedEnemy == null && hadTarget)
            {
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
            }
        }

        void FindNewTargetInRange()
        {
            if (possibleTargets.Count > 0)
            {
                targetedEnemy = possibleTargets[0].gameObject;
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

            Debug.Log(projectile.GetComponent<NetworkHomingMissile>().Target);
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
                    return true;
                }
                return false;

            }
            return false;
        }
    }

}