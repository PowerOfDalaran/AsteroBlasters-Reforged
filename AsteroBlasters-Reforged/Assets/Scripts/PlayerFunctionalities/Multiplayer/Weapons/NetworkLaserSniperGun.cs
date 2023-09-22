using PlayerFunctionality;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class managing the functionalities of laser sniper gun weapon (network version)
    /// </summary>
    public class NetworkLaserSniperGun : NetworkWeapon
    {
        int maxAmmo;
        int currentAmmo;

        [SerializeField] float chargingSlow;
        LineRenderer raycastLaserLineRenderer;

        bool coroutineActive;

        public delegate void OnAmmoValueChange(int current, int maximum);
        public event OnAmmoValueChange onAmmoValueChange;

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
            if (IsOwner)
            {
                // Launching the event on start, so that the text box wouldn't start with "0/0" value
                onAmmoValueChange?.Invoke(currentAmmo, maxAmmo);

                CreateRaycastLaserServerRpc();
            }
        }

        /// <summary>
        /// Server rpc method existing only to call the client rpc - 
        /// only host can use client rpc methods :( 
        /// </summary>
        [ServerRpc]
        void CreateRaycastLaserServerRpc()
        {
            CreateRaycastLaserClientRpc();
        }

        /// <summary>
        /// Client rpc method, which creates the raycast laser prefab attatched to this player character, on every version of the game.
        /// </summary>
        [ClientRpc]
        void CreateRaycastLaserClientRpc()
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
            if (IsOwner)
            {
                // Turning down the slowing effect and destroying the line renderer prefab
                NetworkPlayerController playerController = GetComponent<NetworkPlayerController>();
                playerController.SpeedModifier = 1f;

                RemoveRaycastLaserServerRpc();
            }
        }

        /// <summary>
        /// Server rpc method existing only to call the client rpc - 
        /// only host can use client rpc methods :( 
        /// </summary>
        [ServerRpc]
        void RemoveRaycastLaserServerRpc()
        {
            RemoveRacyastLaserClientRpc();
        }

        /// <summary>
        /// Client rpc method, which destroys the raycast laser object from this player character on every version of the game.
        /// </summary>
        [ClientRpc]
        void RemoveRacyastLaserClientRpc()
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
                gameObject.GetComponent<NetworkPlayerController>().DiscardSecondaryWeapon();
            }
        }

        protected override void AccessHitObject(GameObject hitObject, float charge, long accessingPlayerId)
        {
            // Checking if hit target is a proper enemy
            INetworkHealthSystem networkHealthSystem = hitObject.gameObject.GetComponent<INetworkHealthSystem>();

            if (networkHealthSystem != null)
            {
                // Deal damage based on charged energy
                if (charge <= 6)
                {
                    networkHealthSystem.TakeDamage(1, accessingPlayerId);
                }
                else if (charge < 10)
                {
                    networkHealthSystem.TakeDamage(2, accessingPlayerId);
                }
                else
                {
                    networkHealthSystem.TakeDamage(3, accessingPlayerId);
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
                    // If weapon actually fired, drawing the laser, reducing amount of ammunition and calling the event
                    DrawRaycastLaserServerRpc();
                    currentAmmo -= 1;

                    onAmmoValueChange?.Invoke(currentAmmo, maxAmmo);

                    return true;
                }

                return false;
            }
            return false;
        }

        /// <summary>
        /// Method calling the <c>DrawRaycastLaserClientRpc</c> method from host, since only he can use clients rpc's
        /// </summary>
        [ServerRpc]
        void DrawRaycastLaserServerRpc()
        {
            // Using his raycast distance variable instead of the client one, since the host take a shot and actually know how far away the target is
            DrawRaycastLaserClientRpc(raycastDistance);
        }

        /// <summary>
        /// Method starting the coroutine on every connected player, in order to make the laser visible to everyone
        /// </summary>
        /// <param name="laserDistance">Length of the laser</param>
        [ClientRpc]
        void DrawRaycastLaserClientRpc(float laserDistance)
        {
            StartCoroutine(DrawRaycastLaser(laserDistance));
        }

        /// <summary>
        /// IEnumerator drawing showing and placing correctly the line of laser to display the fired shot
        /// </summary>
        /// <param name="laserDistance">Length of the laser</param>
        IEnumerator DrawRaycastLaser(float laserDistance)
        {
            coroutineActive = true;
            raycastLaserLineRenderer.enabled = true;

           // Setting the laser to reach the hit enemy
            raycastLaserLineRenderer.SetPosition(0, firePoint.position);
            raycastLaserLineRenderer.SetPosition(1, firePoint.position + firePoint.up * laserDistance);

            yield return new WaitForSeconds(0.12f);

            raycastLaserLineRenderer.enabled = false;
            coroutineActive = false;
        }
    }
}
