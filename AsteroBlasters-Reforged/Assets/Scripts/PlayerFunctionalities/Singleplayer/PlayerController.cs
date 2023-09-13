using UnityEngine;
using UnityEngine.InputSystem;
using WeaponSystem;

namespace PlayerFunctionality
{
    /// <summary>
    /// Class responsible for controlling the player character, by moving it, activating sound effects, animations etc.
    /// </summary>
    public class PlayerController : MonoBehaviour, IHealthSystem
    {
        Rigidbody2D myRigidbody2D;
        PlayerControls myPlayerControls;

        [SerializeField] SpaceRifle baseWeapon;
        [SerializeField] Weapon secondaryWeapon;

        [SerializeField] float movementSpeed = 3f;
        [SerializeField] float rotationSpeed = 5.15f;

        /// <summary>
        /// Parameter being used to apply some temporary slowing of fasting effects without affecting the base speed values.
        /// </summary>
        float speedModifier = 1f;
        public float SpeedModifier
        {
            get { return speedModifier; }
            set { speedModifier = value; }
        }


        public int maxHealth = 3;
        public int currentHealth;

        [SerializeField] float currentCharge;
        [SerializeField] float maxCharge = 10f;
        [SerializeField] float chargingSpeed = 10f;

        public bool isChargingWeapon = false;

        public delegate void OnChargeValueChanged(float value);
        public event OnChargeValueChanged onChargeValueChanged;

        public delegate void OnWeaponChanged(WeaponClass weaponClass);
        public event OnWeaponChanged onWeaponChanged;

        void Awake()
        {
            // Assigning values to properties
            myRigidbody2D = GetComponent<Rigidbody2D>();
            myPlayerControls = new PlayerControls();
            baseWeapon = GetComponent<SpaceRifle>();

            currentHealth = maxHealth;
            currentCharge = 0;
        }

        void OnEnable()
        {
            myPlayerControls.Enable();

            // Adding the method for using base weapon to the delegate
            myPlayerControls.PlayerActions.ShootFirstWeapon.performed += UseBaseWeapon;
        }

        void OnDisable()
        {
            myPlayerControls.Disable();

            // Removing the method for using base weapon from the delegate
            myPlayerControls.PlayerActions.ShootFirstWeapon.performed += UseBaseWeapon;
        }

        void Update()
        {
            // Using the second weapon functionality - player can hold and load the attack. 
            // Checking if there is any secondary weapon equipped.
            if (secondaryWeapon != null)
            {
                // In some weapons by doing so, the player can increase damage dealt to opponent
                if (currentCharge >= maxCharge)
                {
                    isChargingWeapon = false;
                    secondaryWeapon.Shoot(currentCharge);
                    currentCharge = 0;
                    onChargeValueChanged?.Invoke(currentCharge);
                }
                else if (myPlayerControls.PlayerActions.ShootSecondaryWeapon.inProgress)
                {
                    isChargingWeapon = true;
                    currentCharge += Time.deltaTime * chargingSpeed;
                    onChargeValueChanged?.Invoke(currentCharge);
                }
                else if (myPlayerControls.PlayerActions.ShootSecondaryWeapon.WasReleasedThisFrame())
                {
                    isChargingWeapon = false;
                    secondaryWeapon.Shoot(currentCharge);
                    currentCharge = 0;
                    onChargeValueChanged?.Invoke(currentCharge);
                }
            }
            
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
        /// Method activating the base weapon in order to fire.
        /// Is added to the "PlayerActions.Shoot.performed" delegate.
        /// Since the base weapon isn't using any charge mechanics, it passes the argument with value 0.
        /// </summary>
        /// <param name="context">Value gathered by input system</param>
        void UseBaseWeapon(InputAction.CallbackContext context)
        {
            baseWeapon.Shoot(0);
        }

        /// <summary>
        /// Method discarding the current secondary weapon
        /// </summary>
        public void DiscardSecondaryWeapon()
        {
            Destroy(secondaryWeapon);
            secondaryWeapon = null;
            onWeaponChanged?.Invoke(WeaponClass.None);
        }

        /// <summary>
        /// Method adding proper weapon class to the game object
        /// </summary>
        /// <param name="weaponClass">Special enumerator representing the weapon class</param>
        /// <param name="weaponProjectile">The projectile the weapon will be using. Currently added to avoid searching for proper prefab in runtime. Need to find better solution though.</param>
        public void PickNewSecondaryWeapon(WeaponClass weaponClass, GameObject weaponProjectile)
        {
            if (secondaryWeapon != null)
            {
                DiscardSecondaryWeapon();
            }

            onWeaponChanged?.Invoke(weaponClass);

            switch (weaponClass)
            {
                case WeaponClass.PlasmaCannon:
                    secondaryWeapon = gameObject.AddComponent<PlasmaCannon>();
                    secondaryWeapon.InstantiateWeapon(weaponProjectile);
                    break;
                case WeaponClass.MissileLauncher:
                    secondaryWeapon = gameObject.AddComponent<MissileLauncher>();
                    secondaryWeapon.InstantiateWeapon(weaponProjectile);
                    break;
                case WeaponClass.LaserSniperGun:
                    secondaryWeapon = gameObject.AddComponent<LaserSniperGun>();
                    secondaryWeapon.InstantiateWeapon(weaponProjectile);
                    break;
                default: 
                    Debug.Log("Unexpected weapon class was given: " +  weaponClass);
                    break;
            }
        }

        /// <summary>
        /// Method moving player character by adding force to its rigidbody2D component.
        /// Is triggered in "FixedUpdate()" method each frame.
        /// </summary>
        /// <param name="context">Value gathered by input system</param>
        void Movement(Vector2 movementVector)
        {
            myRigidbody2D.AddForce(movementVector * movementSpeed * speedModifier, ForceMode2D.Force);
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
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * speedModifier);
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