using PlayerFunctionality;
using Unity.Netcode;
using UnityEngine;

namespace PickableObjects
{
    /// <summary>
    /// Parent class for power ups - pickable objects granting the buff for player that interacted with them (network version).
    /// </summary>
    public class NetworkPowerUp : NetworkBehaviour
    {
        [SerializeField] float powerUpLifeLength = 15f;
        float lifeLengthStatus = 0f;

        float inaccessibilityTime = 1f;
        float inaccessibilityStatus = 0f;

        protected virtual void Start()
        {
            // Setting up the time, at which the power up should be destroyed
            lifeLengthStatus = Time.time + powerUpLifeLength + inaccessibilityTime;

            // Making the power up transparent for short duration of time
            inaccessibilityStatus = Time.time + inaccessibilityTime;
            gameObject.layer = 6;
        }

        private void FixedUpdate()
        {
            // Switching the power up layer to make it pickable
            if (Time.time > inaccessibilityStatus)
            {
                gameObject.layer = 8;
            }

            // Checking if the time limit was surprassed
            if (Time.time > lifeLengthStatus)
            {
                DespawnSelfServerRpc();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            NetworkPlayerController playerController = collision.gameObject.GetComponent<NetworkPlayerController>();

            // Checking if object that collided with power up is player
            if (playerController != null)
            {
                BuffPlayer(playerController);
                DespawnSelfServerRpc();
            }
        }

        /// <summary>
        /// Method granting some kind of advantage to the player that interacted with this power up
        /// </summary>
        /// <param name="playerController"><c>NetworkPlayerController</c> script of player, who picked up the power up</param>
        protected virtual void BuffPlayer(NetworkPlayerController playerController)
        {
            // Implement in child classes
        }

        /// <summary>
        /// Server Rpc method, which despawns the object
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        void DespawnSelfServerRpc()
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }
}