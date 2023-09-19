using PlayerFunctionality;
using UnityEngine;

namespace PickableObjects
{
    /// <summary>
    /// Class managing the power ups healing the person's picking it up (network version)
    /// </summary>
    public class NetworkHealthPowerUp : NetworkPowerUp
    {
        [SerializeField] int amountOfHealing = 2;

        protected override void BuffPlayer(NetworkPlayerController networkPlayerController)
        {
            networkPlayerController.HealPlayerServerRpc(amountOfHealing);
        }
    }
}
