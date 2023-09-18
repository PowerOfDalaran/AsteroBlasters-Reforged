using PlayerFunctionality;
using UnityEngine;

namespace PickableObjects
{
    /// <summary>
    /// Class managing the power ups granting the speed boost to person's picking it up (network version)
    /// </summary>
    public class NetworkSpeedPowerUp : NetworkPowerUp
    {
        [SerializeField] float speedModifier;
        [SerializeField] float buffDuration;

        protected override void BuffPlayer(NetworkPlayerController networkPlayerController)
        {
            networkPlayerController.ModifySpeed(speedModifier, buffDuration);
        }
    }
}
