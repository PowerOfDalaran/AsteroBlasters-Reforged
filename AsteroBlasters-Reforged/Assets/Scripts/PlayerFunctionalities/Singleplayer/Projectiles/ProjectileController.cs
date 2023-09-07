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

            // Checking if colliding object implement health system, if yes, dealing damage to it
            IHealthSystem healthSystem = collision.gameObject.GetComponent<IHealthSystem>();

            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
            }

            Destroy(gameObject);
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