using PlayerFunctionality;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class managing the projectile's functionalities.
    /// </summary>
    public class ProjectileController : MonoBehaviour
    {
        protected Rigidbody2D myRigidbody2D;
        [SerializeField] protected float speed = 20f;
        [SerializeField] protected int damage = 1;

        protected virtual void Awake()
        {
            // Assigning values to class properties
            myRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            // Checking if colliding object implement health system, and if it's the player character
            IHealthSystem healthSystem = collision.gameObject.GetComponent<IHealthSystem>();
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            // The script below got much more complicated because of reservation of the Box Collider component for the targeting zone functionality
            // Therefore it has to be checked if the colliding player wasn't collided with by the targeting zone
            // Need to somehow upgrade the targeting zone functionality later
            if (healthSystem != null && playerController != null && collision is not BoxCollider2D) 
            {
                // If the player got hit in the space ship
                healthSystem.TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (healthSystem != null && playerController == null)
            {
                // If hitten object isn't the player, but can be dealt damage
                healthSystem.TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (healthSystem == null && playerController == null) 
            {
                // If anything else was hit
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Method granting the prefab velocity, to launch it in current direction
        /// </summary>
        public virtual void Launch()
        {
            myRigidbody2D.velocity = transform.up * speed;
        }
    }
}