using GameMapElements;
using PlayerFunctionality;
using Unity.Netcode;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class managing the projectile's functionalities.
    /// This version is also using multiple Netcode methods to allow playing in multiplayer mode.
    /// </summary>
    public class NetworkProjectileController : NetworkBehaviour
    {
        protected Rigidbody2D myRigidbody2D;
        public float speed = 20f;
        public int damage = 1;

        protected virtual void Awake()
        {
            // Assigning values to class properties
            myRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Checking if hit target is a proper enemy
            INetworkHealthSystem networkHealthSystem = collision.GetComponent<INetworkHealthSystem>();

            // Checking if hit object doesn't implement the interface or is player character owned by the shooting player
            if (networkHealthSystem == null || (collision.gameObject.GetComponent<NetworkPlayerController>() != null && collision.gameObject.GetComponent<NetworkObject>().OwnerClientId == OwnerClientId))
            {
                return;
            }
            else if (networkHealthSystem != null)
            {
                networkHealthSystem.TakeDamage(damage, (long)OwnerClientId);
            }
            DespawnSelfServerRpc();
        }

        /// <summary>
        /// Method granting the prefab velocity, to launch it in current direction
        /// </summary>
        public void Launch()
        {
            myRigidbody2D.velocity = transform.up * speed;
        }

        /// <summary>
        /// Method calling the host to despawn this projectile
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        void DespawnSelfServerRpc()
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }
}