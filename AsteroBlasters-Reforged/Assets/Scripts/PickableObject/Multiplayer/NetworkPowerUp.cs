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