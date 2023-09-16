using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using DataStructure;
using NetworkFunctionality;
using Others;
using WeaponSystem;
using PickableObjects;

namespace PlayerFunctionality
{
    /// <summary>
    /// Class responsible for controlling the player character, by moving it, activating sound effects, animations etc.
    /// This version is also using multiple Netcode methods to allow playing in multiplayer mode.
    /// </summary>
    public class NetworkPlayerController : NetworkBehaviour
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

            CameraController.instance.FollowPlayer(transform);


            // Reading current input value for movement and if it's different than zero activate movement and rotation
            Vector2 movementVector = myPlayerControls.PlayerActions.Move.ReadValue<Vector2>();

            if (!movementVector.Equals(new Vector2(0, 0)))
            {
                Movement(movementVector);
                Rotate(movementVector);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {

            // Checking if collision damage should be applied and applying it
            if (!collision.gameObject.CompareTag("NoImpactDamage"))
            {
                if (!IsOwner) return;
                if (collision.gameObject.TryGetComponent(out NetworkPlayerController networkPlayer))
                {
                    //impactVelocity.Value = collision.relativeVelocity.magnitude;

                    var player1 = new PlayerInGameData()
                    {
                        Id = OwnerClientId,
                        ImpactVelocity = collision.relativeVelocity.magnitude
                    };

                    var player2 = new PlayerInGameData()
                    {
                        Id = networkPlayer.OwnerClientId,
                        ImpactVelocity = collision.relativeVelocity.magnitude
                    };
                    ImpactDamageServerRpc(player1, player2);
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
        /// Activates the server rpc method and calls an event.
        /// </summary>
        public void DiscardSecondaryWeapon()
        {
            onWeaponChanged?.Invoke(WeaponClass.None);
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
        #endregion

        #region Taking Damage
        public void TakeDamage(int damage)
        {
            TakeDamageServerRpc(damage);
        }

        public void TakeDamage(int damage, ulong damagingPlayerId)
        {
            TakeDamageServerRpc(damage, damagingPlayerId);
        }

        /// <summary>
        /// Method calling server to deal damage to proper player character and killing it if its health is too low.
        /// </summary>
        /// <param name="damage">Amount of damage you want to deal</param>
        [ServerRpc(RequireOwnership = false)]
        public void TakeDamageServerRpc(int damage, ulong damagingPlayerId = ulong.MaxValue)
        {
            ulong clientId = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(playerIndex).clientId;
            var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<NetworkPlayerController>();

            if (client.currentHealth.Value > 1)
            {
                client.currentHealth.Value -= damage;
            }
            else
            {
                client.currentHealth.Value = 3;
                Die(damagingPlayerId);
            }
        }

        [ServerRpc]
        private void ImpactDamageServerRpc(PlayerInGameData player1, PlayerInGameData player2)
        {
            if (player1.ImpactVelocity > 8)
            {
                Die();
            }
            else if (player1.ImpactVelocity > 6)
            {
                TakeDamageServerRpc(2);
            }
            else if (player1.ImpactVelocity > 5)
            {
                TakeDamageServerRpc(1);
            }
        }
        #endregion

        #region Player Death
        /// <summary>
        /// Method handling the player death (on host)
        /// </summary>
        /// <param name="killerPlayerId">Player id, whose projectile killed this player</param>
        public void Die(ulong killerPlayerId = ulong.MaxValue)
        {
            if (secondaryWeapon != null)
            {
                SpawnWeaponPowerUp(secondaryWeapon);
                DiscardSecondaryWeapon();
            }

            int killingPlayerIndex = MultiplayerGameManager.instance.GetPlayerIndexFromClientId(killerPlayerId);
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

        void SpawnWeaponPowerUp(NetworkWeapon networkWeapon)
        {
            GameObject spawnedPowerUp = Instantiate(weaponPowerUpPrefab, transform.position, Quaternion.identity);
            spawnedPowerUp.GetComponent<NetworkObject>().Spawn();
            spawnedPowerUp.GetComponent<NetworkWeaponPowerUp>().GrantedWeapon = networkWeapon.weaponClass;        }
    }
}