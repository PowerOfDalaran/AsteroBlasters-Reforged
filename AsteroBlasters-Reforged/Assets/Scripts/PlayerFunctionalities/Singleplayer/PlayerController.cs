using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerFunctionality
{
    /// <summary>
    /// Class responsible for controlling the player character, by moving it, activating sound effects, animations etc.
    /// </summary>
    public class PlayerController : MonoBehaviour, IHealthSystem
    {
        Rigidbody2D myRigidbody2D;
        PlayerControls myPlayerControls;
        Weapon myWeapon;

        [SerializeField]
        float movementSpeed = 3f;
        [SerializeField]
        float rotationSpeed = 5.15f;
        public int maxHealth = 3;
        public int currentHealth;

        void Awake()
        {
            // Assigning values to properties
            myRigidbody2D = GetComponent<Rigidbody2D>();
            myPlayerControls = new PlayerControls();
            myWeapon = GetComponent<Weapon>();

            currentHealth = maxHealth;
        }

        void OnEnable()
        {
            // Adding methods to PlayerControls delegates and activating it
            myPlayerControls.Enable();
            myPlayerControls.PlayerActions.Shoot.performed += Shoot;
        }

        void OnDisable()
        {
            // Removing methods from PlayerControls delegates and deactivating it
            myPlayerControls.Disable();
            myPlayerControls.PlayerActions.Shoot.performed -= Shoot;
        }

        private void FixedUpdate()
        {
            // Reading current input value for movement and if it's different than zero activate movement and rotation
            Vector2 movementVector = myPlayerControls.PlayerActions.Move.ReadValue<Vector2>();

            if (!movementVector.Equals(new Vector2(0, 0)))
            {
                Movement(movementVector);
                Rotate(movementVector);
            }

            // Checking if the player is still alive
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Checking if collision damage should be applied and applying it
            if (!collision.gameObject.CompareTag("NoImpactDamage"))
            {
                float impactVelocity = collision.relativeVelocity.magnitude;
                Debug.Log("Impact Damage: " + impactVelocity);

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

        /// <summary>
        /// Method activating the current weapon in order to fire.
        /// Is added to the "PlayerActions.Move.performed" delegate.
        /// </summary>
        /// <param name="context">Value gathered by input system</param>
        void Shoot(InputAction.CallbackContext context)
        {
            myWeapon.Shoot();
        }

        /// <summary>
        /// Method moving player character by adding force to its rigidbody2D component.
        /// Is triggered in "FixedUpdate()" method each frame.
        /// </summary>
        /// <param name="context">Value gathered by input system</param>
        void Movement(Vector2 movementVector)
        {
            myRigidbody2D.AddForce(movementVector * movementSpeed, ForceMode2D.Force);
        }

        /// <summary>
        /// Method rotating player character by creating new desired rotation and then using it to calculate rotation.
        /// Is triggered in "FixedUpdate()" method each frame.
        /// Not that proud of the result, may look for better rotation system later.
        /// </summary>
        /// <param name="movementVector">Value gathered by input system</param>
        void Rotate(Vector2 movementVector)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, movementVector);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
            gameObject.transform.rotation = newRotation;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            Debug.Log("You took " + damage + " damage!");
        }
        public void Die()
        {
            Debug.Log("You died");
        }
    }
}