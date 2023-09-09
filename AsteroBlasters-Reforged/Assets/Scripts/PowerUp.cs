using PlayerFunctionality;
using UnityEngine;

namespace PickableObjects
{
    /// <summary>
    /// Parent class for power ups - pickable objects granting the buff for player that interacted with them.
    /// </summary>
    public class PowerUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            // Checking if object that collided with power up is player
            if (playerController != null)
            {
                BuffPlayer(playerController);
            }
        }

        /// <summary>
        /// Method granting some kind of advantage to the player that interacted with this power up
        /// </summary>
        /// <param name="playerController"><c>PlayerController</c> script of the player, who picked up the power up</param>
        protected virtual void BuffPlayer(PlayerController playerController)
        {
            // Implement in child classes
        }
    }
}
