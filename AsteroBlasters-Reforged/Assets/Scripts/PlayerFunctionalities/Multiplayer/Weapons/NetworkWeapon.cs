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
        [SerializeField] protected GameObject projectilePrefab;

        protected WeaponType type;
        protected WeaponClass weaponClass;

        [SerializeField] protected float fireCooldown = 1;
        protected float cooldownStatus = 0;

        protected float raycastDistance;

        private void Awake()
        {
            firePoint = gameObject.transform.Find("FirePoint");
        }

        public virtual void InstantiateWeapon()
        {
            // Implement in child classes
        }

        [ServerRpc]
        public void InstantiateWeaponServerRpc(ServerRpcParams serverRpcParams = default)
        {

        }

        public virtual bool Shoot(float charge)
        {
            if (Time.time > cooldownStatus)
            {
                cooldownStatus = Time.time + fireCooldown;

                if (type == WeaponType.ProjectileBased) 
                {
                    ShootServerRpc(charge);
                    return true;
                }
            }
            return false;
        }

        protected virtual void AccessCreatedProjectile(GameObject projectile)
        {
            // Implement in child classes
        }

        /// <summary>
        /// Method, which using ServerRpc, creates new projectile, spawn and launch it, if fire cooldown has passed.
        /// </summary>
        [ServerRpc]
        private void ShootServerRpc(float charge, ServerRpcParams serverRpcParams = default)
        {
            ulong projectileOwner = serverRpcParams.Receive.SenderClientId;

            GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            AccessCreatedProjectile(newProjectile);

            newProjectile.GetComponent<NetworkObject>().SpawnWithOwnership(projectileOwner);
            newProjectile.GetComponent<NetworkProjectileController>().Launch();
        }
    }
}