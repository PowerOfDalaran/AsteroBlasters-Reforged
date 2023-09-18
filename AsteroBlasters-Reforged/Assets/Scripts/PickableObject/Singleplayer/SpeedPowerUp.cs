using PlayerFunctionality;
using UnityEngine;

namespace PickableObjects
{
    /// <summary>
    /// Class managing the power ups granting the speed boost to person's picking it up
    /// </summary>
    public class SpeedPowerUp : PowerUp
    {
        [SerializeField] float speedModifier;
        [SerializeField] float buffDuration;

        protected override void BuffPlayer(PlayerController playerController)
        {
            playerController.ModifySpeed(speedModifier, buffDuration);
            Destroy(gameObject);
        }
    }
}
