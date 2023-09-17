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
        protected Transform firePoint;
        [SerializeField] protected GameObject projectilePrefab;

        protected WeaponType type;
        public WeaponClass weaponClass;

        [SerializeField] protected float fireCooldown = 1;
        protected float cooldownStatus = 0;

        protected float raycastDistance;

        private void Awake()
        {
            firePoint = gameObject.transform.Find("FirePoint");
        }

        /// <summary>
        /// Method assigning the proper values to properties.
        /// Implemented in child classes.
        /// </summary>
        public virtual void InstantiateWeapon()
        {
            // Implement in child classes
        }

        /// <summary>
        /// Method calling the host to assign proper values to properties on this player character as well.
        /// </summary>
        /// <param name="serverRpcParams"></param>
        [ServerRpc]
        public void InstantiateWeaponServerRpc(ServerRpcParams serverRpcParams = default)
        {
            InstantiateWeapon();
        }

        /// <summary>
        /// Method calling the server to execute the "ShootServerRpc" method if cooldown for weapon has passed.
        /// </summary>
        /// <param name="charge">The value representing how long the shot was charged before firing.</param>
        /// <returns>Whether the weapon actually fired or not (if was called while cooldown hasn't passed it returns false)</returns>
        public virtual bool Shoot(float charge)
        {
            if (Time.time > cooldownStatus)
            {
                cooldownStatus = Time.time + fireCooldown;

                ShootServerRpc(charge);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method called by the host in "ShootServerRpc" method, so that the created projectile can be accessed.
        /// This method function in order to get around the fact that Server Rpc methods must be void.
        /// </summary>
        /// <param name="projectile">Projectile creted by the server</param>
        protected virtual void AccessCreatedProjectile(GameObject projectile)
        {
            // Implement in child classes
        }

        /// <summary>
        /// Method called by the host in <c>ShootServerRpc</c> method, so that object hit by the raycast can be accessed.
        /// This method function in order to get around the fact that Server Rpc methods must be void.
        /// </summary>
        /// <param name="hitObject">Game object hit by the raycast</param>
        /// <param name="charge">Value representing how long the shot was charged before firing</param>
        /// <param name="accessingPlayerId">Id of the player, who shot the raycast</param>
        protected virtual void AccessHitObject(GameObject hitObject, float charge, ulong accessingPlayerId)
        {
            // Implement in child classes
        }

        /// <summary>
        /// Method, which using ServerRpc, creates new projectile, spawn and launch it, if fire cooldown has passed.
        /// </summary>
        [ServerRpc]
        private void ShootServerRpc(float charge, ServerRpcParams serverRpcParams = default)
        {
            ulong callerId = serverRpcParams.Receive.SenderClientId;

            // If the shooting weapon is projectile based, creating new projectile, spawning and launching it
            if (type == WeaponType.ProjectileBased)
            {
                GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

                AccessCreatedProjectile(newProjectile);

                newProjectile.GetComponent<NetworkObject>().SpawnWithOwnership(callerId);
                newProjectile.GetComponent<NetworkProjectileController>().Launch();
            }
            // If weapon is raycast based, firing the raycast and accessing the hit object
            else if (type == WeaponType.RaycastBased)
            {
                RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.up, 100f, 7);
                
                if (hitInfo)
                {
                    raycastDistance = hitInfo.distance;
                    AccessHitObject(hitInfo.transform.gameObject, charge, callerId);
                }
                else
                {
                    raycastDistance = -1f;
                }
            }
        }
    }
}