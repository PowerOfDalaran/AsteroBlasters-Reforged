using PlayerFunctionality;
using UnityEngine;

namespace PickableObjects
{
    /// <summary>
    /// Parent class for power ups - pickable objects granting the buff for player that interacted with them.
    /// </summary>
    public class PowerUp : MonoBehaviour
    {
        [SerializeField] float powerUpLifeLength = 15f;
        [SerializeField] float lifeLengthStatus = 0f;

        private void Start()
        {
            // Setting up the time, at which the power up should be destroyed
            lifeLengthStatus = Time.time + powerUpLifeLength;
        }

        private void FixedUpdate()
        {
            // Checking if the time limit was surprassed
            if (Time.time > lifeLengthStatus)
            {
                Destroy(gameObject);
            }
        }

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
