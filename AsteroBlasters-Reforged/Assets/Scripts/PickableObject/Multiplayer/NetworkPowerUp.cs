using PlayerFunctionality;
using Unity.Netcode;
using UnityEngine;

namespace PickableObjects
{
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

        protected virtual void BuffPlayer(NetworkPlayerController playerController)
        {
            // Implement in child classes
        }

        [ServerRpc(RequireOwnership = false)]
        void DespawnSelfServerRpc()
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }
}