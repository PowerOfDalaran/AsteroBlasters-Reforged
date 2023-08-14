using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using DataStructure;
using NetworkFunctionality;
using GameManager;
using Others;

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
        NetworkWeapon myWeapon;

        [SerializeField] float movementSpeed = 3f;
        [SerializeField] float rotationSpeed = 720f;
        public int playerIndex;

        public NetworkVariable<int> maxHealth = new NetworkVariable<int>();
        public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

        #region Build-in methods
        void Awake()
        {
            // Assigning values to properties
            myRigidbody2D = GetComponent<Rigidbody2D>();
            myWeapon = GetComponent<NetworkWeapon>();
            mySpriteRenderer = GetComponent<SpriteRenderer>();
            myPlayerControls = new PlayerControls();

            maxHealth.Value = 3;
            currentHealth = maxHealth;
        }

        void Start()
        {
            // Setting up color of the player
            PlayerData playerData = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(playerIndex);
            Color myColor = MultiplayerGameManager.instance.GetPlayerColor(playerData.colorId);
            mySpriteRenderer.color = myColor;
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
        /// Method activating the current weapon in order to fire.
        /// Is added to the "PlayerActions.Move.performed" delegate.
        /// </summary>
        /// <param name="context">Value gathered by input system</param>
        void Shoot(InputAction.CallbackContext context)
        {
            // Deciding whether the rest of method should be activated
            if (!IsOwner)
            {
                return;
            }
            myWeapon.ShootServerRpc();
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
            //Debug.Log("Impact Damage player1: " + player1.ImpactVelocity);
            //Debug.Log("Impact Damage player2: " + player2.ImpactVelocity);
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
        public void Die(ulong killerPlayerId = ulong.MaxValue)
        {
            int killingPlayerIndex = MultiplayerGameManager.instance.GetPlayerIndexFromClientId(killerPlayerId);

            DieClientRpc();
            DeathmatchGameManager.instance.AddKillCountServerRpc(killingPlayerIndex);
        }

        /// <summary>
        /// Method called on every client (host too) and activating death on this player character object.
        /// Currently just reset the game object position and velocity.
        /// </summary>
        [ClientRpc]
        public void DieClientRpc()
        {
            myRigidbody2D.position = new Vector3(0f, 0f, 0f);
            myRigidbody2D.velocity = Vector2.zero;
        }
        #endregion
    }
}