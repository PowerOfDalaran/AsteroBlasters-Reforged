using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using DataStructure;
using NetworkFunctionality;
using Others;
using WeaponSystem;
using PickableObjects;
using System.Collections;

namespace PlayerFunctionality
{
    /// <summary>
    /// Class responsible for controlling the player character, by moving it, activating sound effects, animations etc.
    /// This version is also using multiple Netcode methods to allow playing in multiplayer mode.
    /// </summary>
    public class NetworkPlayerController : NetworkBehaviour, INetworkHealthSystem
    {
        Rigidbody2D myRigidbody2D;
        SpriteRenderer mySpriteRenderer;
        PlayerControls myPlayerControls;

        [SerializeField] GameObject weaponPowerUpPrefab;
        [SerializeField] NetworkWeapon[] weaponArray = new NetworkWeapon[3];

        [SerializeField] NetworkSpaceRifle baseWeapon;
        [SerializeField] NetworkWeapon secondaryWeapon;

        [SerializeField] float currentCharge;
        [SerializeField] float maxCharge;

        [SerializeField] float chargingSpeed;
        [SerializeField] public bool isChargingWeapon;

        [SerializeField] float movementSpeed = 3f;
        [SerializeField] float rotationSpeed = 5.15f;

        [SerializeField] float speedModifier = 1f;
        public float SpeedModifier
        {
            get { return speedModifier; }
            set { speedModifier = value; }
        }

        public int playerIndex;

        public delegate void OnPlayerDeath(int killedPlayerIndex, int killingPlayerIndex);
        public static event OnPlayerDeath onPlayerDeath;

        public delegate void OnChargeValueChanged(float value);
        public event OnChargeValueChanged onChargeValueChanged;

        public delegate void OnWeaponChanged(WeaponClass weaponClass);
        public event OnWeaponChanged onWeaponChanged;

        public NetworkVariable<int> maxHealth = new NetworkVariable<int>();
        public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

        public NetworkVariable<int> maxShield = new NetworkVariable<int>();
        public NetworkVariable<int> currentShield = new NetworkVariable<int>();

        public NetworkVariable<Vector3> spawnPosition = new NetworkVariable<Vector3>();

        #region Build-in methods
        void Awake()
        {
            // Assigning values to properties
            myRigidbody2D = GetComponent<Rigidbody2D>();
            baseWeapon = GetComponent<NetworkSpaceRifle>();
            mySpriteRenderer = GetComponent<SpriteRenderer>();
            myPlayerControls = new PlayerControls();

            maxHealth.Value = 3;
            currentHealth.Value = maxHealth.Value;

            maxShield.Value = 2;
            currentShield.Value = 0;

            chargingSpeed = 8f;
            isChargingWeapon = false;

            maxCharge = 10f;
            currentCharge = 0f;
        }

        void OnEnable()
        {
            // Adding methods to PlayerControls delegates and activating it
            myPlayerControls.Enable();
            myPlayerControls.PlayerActions.ShootFirstWeapon.performed += UseBaseWeapon;
        }

        void OnDisable()
        {
            // Removing methods from PlayerControls delegates and deactivating it
            myPlayerControls.Disable();
            myPlayerControls.PlayerActions.ShootFirstWeapon.performed -= UseBaseWeapon;
        }

        void Start()
        {
            // Setting up color of the player
            PlayerNetworkData playerData = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(playerIndex);
            Color myColor = MultiplayerGameManager.instance.GetPlayerColor(playerData.colorId);
            mySpriteRenderer.color = myColor;

            // If this player character is owned by the local client, setting up the camera to follow them
            if (IsOwner)
            {
                CameraController.instance.FollowPlayer(transform);
            }
        }

        private void Update()
        {
            // Deciding whether the rest of method should be activated
            if (!IsOwner)
            {
                return;
            }

            // Using the second weapon functionality - player can hold and load the attack. 
            // Checking if there is any secondary weapon equipped.
            if (secondaryWeapon != null)
            {
                // In some weapons by doing so, the player can increase damage dealt to opponent
                if (currentCharge >= maxCharge)
                {
                    // IN CASE THE CHARGE METER REACHES MAXIMUM
                    // Checking if shot was actually fired, if so the charge counter is set back to 0, otherwise the weapon keeps charging
                    bool shotTaken = secondaryWeapon.Shoot(currentCharge);
                    if (shotTaken)
                    {
                        isChargingWeapon = false;
                        currentCharge = 0;
                        onChargeValueChanged?.Invoke(currentCharge);
                    }
                }
                else if (myPlayerControls.PlayerActions.ShootSecondaryWeapon.inProgress)
                {
                    // IN CASE THE SHOOT BUTTON IS STILL HELD
                    isChargingWeapon = true;
                    currentCharge += Time.deltaTime * chargingSpeed;
                    onChargeValueChanged?.Invoke(currentCharge);
                }
                else if (myPlayerControls.PlayerActions.ShootSecondaryWeapon.WasReleasedThisFrame())
                {
                    // IN CASE THE BUTTON WAS RELEASED
                    isChargingWeapon = false;
                    secondaryWeapon.Shoot(currentCharge);
                    currentCharge = 0;
                    onChargeValueChanged?.Invoke(currentCharge);
                }
            }
        }

        private void FixedUpdate()
        {
            // Deciding whether the rest of method should be activated
            if (!IsOwner)
            {
                return;
            }

            // Reading current input value for movement and if it's different than zero activate movement and rotation
            Vector2 rotationVector = myPlayerControls.PlayerActions.Rotate.ReadValue<Vector2>();

            if (!rotationVector.Equals(new Vector2(0, 0)))
            {
                Rotate(rotationVector);
            }

            if (myPlayerControls.PlayerActions.Move.inProgress)
            {
                Movement();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {

            // Checking if collision damage should be applied and applying it
            if (!collision.gameObject.CompareTag("NoImpactDamage"))
            {
                // Checking if code is run by the owner of the player character
                if (!IsOwner) return;

                // Calculating velocity and assigning the proper amount of damage
                float impactVelocity = collision.relativeVelocity.magnitude;
                int impactDamage = 0;

                if (impactVelocity > 7)
                {
                    impactDamage = 5;
                }
                else if (impactVelocity > 5)
                {
                    impactDamage = 2;
                }
                else if (impactVelocity > 3)
                {
                    impactDamage = 1;
                }

                // Applying the damage to self...
                TakeDamage(impactDamage);

                // ...and to colliding player character, if an object is indeed the other player's ship
                NetworkPlayerController collidingNetworkPlayerController = collision.gameObject.GetComponent<NetworkPlayerController>();
                if (collidingNetworkPlayerController != null && impactDamage > 0)
                {
                    collidingNetworkPlayerController.TakeDamage(impactDamage);
                }
            }
        }
        #endregion

        #region Set Player Data
        /// <summary>
        /// Method assigning given player index to this instance of player character
        /// </summary>
        /// <param name="givenIndex">Player index you this player to store</param>
        [ClientRpc]
        public void SetMyIndexClientRpc(int givenIndex)
        {
            playerIndex = givenIndex;
        }
        #endregion

        #region Player Actions
        /// <summary>
        /// Method activating the base weapon in order to fire.
        /// Is added to the "PlayerActions.ShootFirstWeapon.performed" delegate.
        /// </summary>
        /// <param name="context">Value gathered by input system</param>
        void UseBaseWeapon(InputAction.CallbackContext context)
        {
            // Deciding whether the rest of method should be activated
            if (!IsOwner)
            {
                return;
            }
            baseWeapon.Shoot(0);
        }

        /// <summary>
        /// Method being activated if the current secondary weapon should be discarded.
        /// </summary>
        public void DiscardSecondaryWeapon()
        {
            DiscardSecondaryWeaponServerRpc();
        }

        /// <summary>
        /// Method, which only purpose is firing the Client Rpc method. It exist purely because only host can activate the Client Rpc's.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        void DiscardSecondaryWeaponServerRpc()
        {
            DiscardSecondaryWeaponClientRpc();
        }

        /// <summary>
        /// Method launched by every connected player, discarding the weapon from that player character.
        /// </summary>
        /// <param name="clientRpcParams"></param>
        [ClientRpc]
        void DiscardSecondaryWeaponClientRpc(ClientRpcParams clientRpcParams = default)
        {
            // Calling the onWeaponChanged event if client running method is the owner of this player character
            if (IsOwner)
            {
                onWeaponChanged?.Invoke(WeaponClass.None);
            }

            if (secondaryWeapon != null)
            {
                secondaryWeapon.enabled = false;
                secondaryWeapon = null;
            }
        }

        /// <summary>
        /// Method activating the weapon with given class on the player character.
        /// </summary>
        /// <param name="weaponClass">The class of weapon, which is supposed to be equipped</param>
        public void PickNewSecondaryWeapon(WeaponClass weaponClass)
        {
            // Checking if old weapon shouldn't be discarded first
            if (secondaryWeapon != null)
            {
                DiscardSecondaryWeapon();
            }

            // Activating the event
            onWeaponChanged?.Invoke(weaponClass);

            // Changing the secondary weapon based on given class
            switch (weaponClass)
            {
                case WeaponClass.PlasmaCannon:
                    ChangeSecondaryWeaponServerRpc(0);
                    break;
                case WeaponClass.MissileLauncher:
                    ChangeSecondaryWeaponServerRpc(1);
                    break;
                case WeaponClass.LaserSniperGun:
                    ChangeSecondaryWeaponServerRpc(2);
                    break;
                default:
                    Debug.Log("Unexpected weapon class was given: " + weaponClass);
                    break;
            }
        }

        /// <summary>
        /// Method calling the host to activate the Client Rpc method, since only he can do that :|
        /// </summary>
        /// <param name="weaponId"></param>
        [ServerRpc(RequireOwnership = false)]
        void ChangeSecondaryWeaponServerRpc(int weaponId)
        {
            ChangeSecondaryWeaponClientRpc(weaponId);
        }

        /// <summary>
        /// Method changing the weapon of this player character to the one with given index.
        /// </summary>
        /// <param name="weaponId">Index of weapon in the weapon array</param>
        [ClientRpc]
        void ChangeSecondaryWeaponClientRpc(int weaponId)
        {
            secondaryWeapon = weaponArray[weaponId];
            secondaryWeapon.InstantiateWeapon();
            secondaryWeapon.enabled = true;
        }

        /// <summary>
        /// Method moving player character by adding force to its rigidbody2D component.
        /// Is triggered in "FixedUpdate()" method each frame.
        /// </summary>
        void Movement()
        {
            myRigidbody2D.AddForce(transform.up * movementSpeed * speedModifier, ForceMode2D.Force);
        }

        /// <summary>
        /// Method rotating player character by creating new desired rotation and then using it to calculate rotation.
        /// Is triggered in "FixedUpdate()" method each frame.
        /// Not that proud of the result, may look for better rotation system later.
        /// </summary>
        /// <param name="movementVector">Value gathered by input system</param>
        void Rotate(Vector2 rotationVector)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, rotationVector);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * speedModifier);
            gameObject.transform.rotation = newRotation;
        }
        #endregion

        #region Taking Damage
        public void TakeDamage(int damage, long damagingPlayerId = -1)
        {
            TakeDamageServerRpc(damage, damagingPlayerId);
        }

        /// <summary>
        /// Method calling server to deal damage to proper player character and killing it if its health is too low.
        /// </summary>
        /// <param name="damage">Amount of damage you want to deal</param>
        [ServerRpc(RequireOwnership = false)]
        public void TakeDamageServerRpc(int damage, long damagingPlayerId)
        {
            ulong clientId = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(playerIndex).clientId;
            var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<NetworkPlayerController>();

            // If shield is available, it will block incoming hit, without passing any excess damage
            if (currentShield.Value > 0)
            {
                currentShield.Value -= damage;

                // If shield was destroyed, setting it to 0 in order to not end up with negative shield
                if (currentShield.Value < 0)
                {
                    currentShield.Value = 0;
                }
            }
            else
            {
                // If there was no shield, dealing damage to the player...
                if (client.currentHealth.Value > 1)
                {
                    client.currentHealth.Value -= damage;
                }
                else
                {
                    //...or killing him, if he won't be able to survive the damage
                    client.currentHealth.Value = 3;
                    Die(damagingPlayerId);
                }
            }
        }
        #endregion

        #region Player Death
        /// <summary>
        /// Method handling the player death (on host)
        /// </summary>
        /// <param name="killerPlayerId">Player id, whos killed this player (-1 if the player died on their own)</param>
        public void Die(long killerPlayerId)
        {
            if (secondaryWeapon != null)
            {
                SpawnWeaponPowerUp(secondaryWeapon);
                DiscardSecondaryWeapon();
            }

            // Assigning the killing player index (-1 if the player died on their own)
            int killingPlayerIndex = killerPlayerId >= 0 ? MultiplayerGameManager.instance.GetPlayerIndexFromClientId((ulong)killerPlayerId) : -1;

            // Resetting the current health, activating client rpc and activating the event
            currentHealth = maxHealth;
            DieClientRpc();
            onPlayerDeath?.Invoke(playerIndex, killingPlayerIndex);
        }

        /// <summary>
        /// Method called on every instance of the game and activating death on this player character object.
        /// Currently just reset the game object position and velocity.
        /// </summary>
        [ClientRpc]
        public void DieClientRpc()
        {
            myRigidbody2D.position = spawnPosition.Value;
            myRigidbody2D.velocity = Vector2.zero;
        }
        #endregion

        /// <summary>
        /// Method creating new weapon power up and spawning it.
        /// Activated from server after player death.
        /// </summary>
        /// <param name="networkWeapon">Class of weapon, which the new power up will grant</param>
        void SpawnWeaponPowerUp(NetworkWeapon networkWeapon)
        {
            GameObject spawnedPowerUp = Instantiate(weaponPowerUpPrefab, transform.position, Quaternion.identity);
            spawnedPowerUp.GetComponent<NetworkObject>().Spawn();
            spawnedPowerUp.GetComponent<NetworkWeaponPowerUp>().GrantedWeapon = networkWeapon.weaponClass;        
        }

        /// <summary>
        /// Server rpc method healing this player by given amount.
        /// If the max health property would be crossed, healing them up to his max health.
        /// </summary>
        /// <param name="amountOfHealing">Amount of health that should be restored</param>
        [ServerRpc]
        public void HealPlayerServerRpc(int amountOfHealing)
        {
            currentHealth.Value = currentHealth.Value + amountOfHealing > maxHealth.Value ? maxHealth.Value : currentHealth.Value + amountOfHealing;
        }

        /// <summary>
        /// Server rpc method giving shield to the person's picking it up by the grantedShield property, if it wouldn't cross the maxShield amount.
        /// Otherwise increasing their shield up to their maximum shield.        
        /// </summary>
        /// <param name="shieldValue">Amount of shield that should be granted</param>
        [ServerRpc]
        public void GainShieldServerRpc(int shieldValue)
        {
            currentShield.Value = currentShield.Value + shieldValue > maxShield.Value ? maxShield.Value : currentShield.Value + shieldValue;
        }

        /// <summary>
        /// Method activating the Modify Speed Coroutine coroutine.
        /// </summary>
        /// <param name="modifier">Modifier, which is multiplied by the player character speed</param>
        /// <param name="duration">Duration of the speed modification</param>
        public void ModifySpeed(float modifier, float duration)
        {
            StartCoroutine(ModifySpeedCoroutine(modifier, duration));
        }

        /// <summary>
        /// Method changing the speed modifier parameter of player character for given amount of seconds
        /// </summary>
        /// <param name="modifier">Modifier, which is multiplied by the player character speed</param>
        /// <param name="duration">Duration of the speed modification</param>
        /// <returns></returns>
        IEnumerator ModifySpeedCoroutine(float modifier, float duration)
        {
            speedModifier *= modifier;

            yield return new WaitForSeconds(duration);

            speedModifier /= modifier;
        }
    }
}