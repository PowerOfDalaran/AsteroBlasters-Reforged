using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GameMapElements
{
    public class SafeZoneController : NetworkBehaviour
    {
        public void SetUpSafeZone(ulong zoneOwner)
        {
            NetworkObject.ChangeOwnership(zoneOwner);
            UpdateSafeZoneClientRpc();
        }

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
