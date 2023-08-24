using Unity.Netcode;
using UnityEngine;

namespace GameMapElements
{
    /// <summary>
    /// Class managing the Safe Zone game object behaviour 
    /// </summary>
    public class SafeZoneController : NetworkBehaviour
    {
        /// <summary>
        /// Method assigning given id to OwnerId property of NetworkObject and activating the update method
        /// </summary>
        /// <param name="zoneOwnerId">Id of player, this zone is supposed to be own by</param>
        public void SetUpSafeZone(ulong zoneOwnerId)
        {
            NetworkObject.ChangeOwnership(zoneOwnerId);
            UpdateSafeZoneClientRpc();
        }

        /// <summary>
        /// Method activating on every connected player, which updates its layer and changes the color (if the activating client owns this zone)
        /// </summary>
        [ClientRpc]
        void UpdateSafeZoneClientRpc()
        {
            if (IsOwner)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 0.5f);
                gameObject.layer = 6;
            }
        }
    }
}
