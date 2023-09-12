using PlayerFunctionality;
using Unity.Netcode;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class responsible for firing projectiles from certain position.
    /// This version is also using multiple Netcode methods to allow playing in multiplayer mode.
    /// </summary>
    public class NetworkWeapon : NetworkBehaviour
    {
        Transform firePoint;
        [SerializeField] GameObject projectilePrefab;

        protected WeaponType type;
        protected WeaponClass weaponClass;

        [SerializeField] protected float fireCooldown = 1;
        protected float cooldownStatus = 0;

        protected float raycastDistance;

        private void Awake()
        {
            firePoint = gameObject.transform.Find("FirePoint");
        }

        public virtual void InstantiateWeapon(GameObject projectile = null)
        {
            // Implement in child classes
        }

        public virtual void Shoot(float charge)
        {
            if (Time.time > cooldownStatus)
            {
                cooldownStatus = Time.time + fireCooldown;

                if (type == WeaponType.ProjectileBased) 
                {
                    ShootServerRpc(charge);
                }
            }
        }

        /// <summary>
        /// Method, which using ServerRpc, creates new projectile, spawn and launch it, if fire cooldown has passed.
        /// </summary>
        [ServerRpc]
        private void ShootServerRpc(float charge, ServerRpcParams serverRpcParams = default)
        {
            ulong projectileOwner = serverRpcParams.Receive.SenderClientId;

            GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            newProjectile.GetComponent<NetworkObject>().SpawnWithOwnership(projectileOwner);
            newProjectile.GetComponent<NetworkProjectileController>().Launch();
        }
    }
}