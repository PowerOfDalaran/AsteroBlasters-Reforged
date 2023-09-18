using PlayerFunctionality;
using UnityEngine;

namespace PickableObjects
{
    public class NetworkHealthPowerUp : NetworkPowerUp
    {
        [SerializeField] int amountOfHealing;

        protected override void BuffPlayer(NetworkPlayerController networkPlayerController)
        {
            networkPlayerController.HealPlayerServerRpc(amountOfHealing);
        }
    }
}
