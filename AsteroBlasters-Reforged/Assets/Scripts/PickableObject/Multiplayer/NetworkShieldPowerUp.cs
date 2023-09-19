using PlayerFunctionality;
using UnityEngine;

namespace PickableObjects
{
    /// <summary>
    /// Class managing the power ups granting shield to the person's picking it up (network version)
    /// </summary>
    public class NetworkShieldPowerUp : NetworkPowerUp
    {
        [SerializeField] int grantedShield = 1;

        protected override void BuffPlayer(NetworkPlayerController playerController)
        {
            playerController.GainShieldServerRpc(grantedShield);
        }
    }
}
