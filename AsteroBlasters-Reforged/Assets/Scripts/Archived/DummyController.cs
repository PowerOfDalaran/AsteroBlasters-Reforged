using UnityEngine;
using PlayerFunctionality;
using WeaponSystem;

namespace Archive
{
    /// <summary>
    /// Class controlling the behaviour of a dummy.
    /// </summary>
    public class DummyController : MonoBehaviour, IHealthSystem
    {
        Weapon myWeapon;

        [SerializeField]
        int maxHealth = 2;
        [SerializeField]
        int currentHealth;

        public void Die()
        {
            Destroy(gameObject);
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
        }

        void Awake()
        {
            // Assigning values to properties
            myWeapon = gameObject.GetComponent<Weapon>();
            currentHealth = maxHealth;
        }

        void FixedUpdate()
        {
            // Checking if dummy is still alive
            if (currentHealth <= 0)
            {
                Die();
            }

            if (myWeapon != null)
            {
                myWeapon.Shoot(0f);
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            // Checking if collision damage should be applied and applying it
            if (!collision.gameObject.CompareTag("NoImpactDamage"))
            {
                float impactVelocity = collision.relativeVelocity.magnitude;

                if (impactVelocity > 8)
                {
                    Die();
                }
                else if (impactVelocity > 6)
                {
                    TakeDamage(2);
                }
                else if (impactVelocity > 5)
                {
                    TakeDamage(1);
                }
            }
        }
    }
}