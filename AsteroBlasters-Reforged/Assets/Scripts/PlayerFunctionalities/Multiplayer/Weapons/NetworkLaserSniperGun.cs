using PlayerFunctionality;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;

namespace WeaponSystem
{
    public class NetworkLaserSniperGun : NetworkWeapon
    {
        int maxAmmo;
        int currentAmmo;

        [SerializeField] float chargingSlow;
        LineRenderer raycastLaserLineRenderer;

        bool coroutineActive;

        public override void InstantiateWeapon()
        {
            // Assigning the values to the properties
            type = WeaponType.RaycastBased;
            weaponClass = WeaponClass.LaserSniperGun;
            fireCooldown = 1.5f;

            chargingSlow = 0.64f;
            maxAmmo = 5;
            currentAmmo = maxAmmo;
            coroutineActive = false;
        }

        private void OnEnable()
        {
            // Setting up the line renderer prefab
            GameObject createdRaycastLaser = Instantiate(projectilePrefab);
            createdRaycastLaser.transform.parent = transform;

            createdRaycastLaser.transform.position = Vector2.zero;
            raycastLaserLineRenderer = createdRaycastLaser.GetComponent<LineRenderer>();

            raycastLaserLineRenderer.enabled = false;
        }

        private void OnDisable()
        {
            Destroy(gameObject.transform.Find("RaycastLaser(Clone)").gameObject);
        }

        private void FixedUpdate()
        {
            // Slowing layer down, while he's charging the shot
            NetworkPlayerController playerController = GetComponent<NetworkPlayerController>();
            playerController.SpeedModifier = playerController.isChargingWeapon ? chargingSlow : 1f;

            //Checking if there's any ammo left, and discarding the weapon if not
            if (currentAmmo <= 0 && !coroutineActive)
            {
                gameObject.GetComponent<NetworkPlayerController>().DiscardSecondaryWeaponClientRpc();
            }
        }

        protected override void AccessHitObject(GameObject hitObject, float charge, ulong accessingPlayerId)
        {
            // Checking if hit target is a proper enemy
            NetworkPlayerController playerController = hitObject.GetComponent<NetworkPlayerController>();

            if (playerController != null)
            {
                // Deal damage based on charged energy
                if (charge <= 6)
                {
                    playerController.TakeDamage(1, accessingPlayerId);
                }
                else if (charge < 10)
                {
                    playerController.TakeDamage(2, accessingPlayerId);
                }
                else
                {
                    playerController.TakeDamage(3, accessingPlayerId);
                }
            }
        }

        public override bool Shoot(float charge)
        {
            if (currentAmmo > 0)
            {
                bool weaponFired = base.Shoot(charge);

                if (weaponFired)
                {
                    DrawRaycastLaserClientRpc();
                    currentAmmo -= 1;
                    return true;
                }

                return false;
            }
            return false;
        }

        [ClientRpc]
        void DrawRaycastLaserClientRpc()
        {
            StartCoroutine(DrawRaycastLaser());
        }
        
        IEnumerator DrawRaycastLaser()
        {
            coroutineActive = true;
            raycastLaserLineRenderer.enabled = true;

            if ( raycastDistance > 0)
            {
                // Setting the laser to reach the hit enemy
                raycastLaserLineRenderer.SetPosition(0, firePoint.position);
                raycastLaserLineRenderer.SetPosition(1, firePoint.position + firePoint.up * raycastDistance);
            }
            else
            {
                // Setting the laser to reach some distance before vanishing
                raycastLaserLineRenderer.SetPosition(0, firePoint.position);
                raycastLaserLineRenderer.SetPosition(1, firePoint.position + firePoint.up * 100);
            }

            yield return new WaitForSeconds(0.12f);

            raycastLaserLineRenderer.enabled = false;
            coroutineActive = false;
        }
    }
}
