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
            // Checking if colliding object is a player character
            NetworkPlayerController networkPlayerController = collision.gameObject.GetComponent<NetworkPlayerController>();

            NetworkAsteroidController networkAsteroidController = collision.gameObject.GetComponent<NetworkAsteroidController>();

            // If not, then destroying projectile
            if (networkPlayerController == null && networkAsteroidController == null)
            {
                DespawnSelfServerRpc();
            }
            // if so, and player hit isn't the owner of the projectile, then dealing damage to him, before deleting the projectile
            else if (collision.gameObject.GetComponent<NetworkObject>().OwnerClientId != OwnerClientId && networkPlayerController != null)
            {
                networkPlayerController.TakeDamage(damage, OwnerClientId);
                DespawnSelfServerRpc();
            }
            else if(networkAsteroidController != null)
            {
                networkAsteroidController.TakeDamage(damage);
                DespawnSelfServerRpc();
            }
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