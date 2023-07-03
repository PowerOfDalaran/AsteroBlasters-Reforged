using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

/// <summary>
/// Class responsible for controlling the player character, by moving it, activating sound effects, animations etc.
/// This version is also using multiple Netcode methods to allow playing in multiplayer mode.
/// </summary>
public class NetworkPlayerController : NetworkBehaviour, IHealthSystem
{
    Rigidbody2D myRigidbody2D;
    PlayerControls myPlayerControls;
    NetworkWeapon myWeapon;

    [SerializeField]
    float movementSpeed = 3f;
    [SerializeField]
    float rotationSpeed = 720;

    public NetworkVariable<int> maxHealth = new NetworkVariable<int>();
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    // Start position for each player
    private Vector2 startPosition;

    private void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myPlayerControls = new PlayerControls();
        myWeapon = GetComponent<NetworkWeapon>();
        maxHealth.Value = 3;
        currentHealth.Value = maxHealth.Value;

        // Store the start position of the player
        startPosition = myRigidbody2D.position;
    }

    private void OnEnable()
    {
        myPlayerControls.Enable();
        myPlayerControls.PlayerActions.Shoot.performed += Shoot;
    }

    private void OnDisable()
    {
        myPlayerControls.Disable();
        myPlayerControls.PlayerActions.Shoot.performed -= Shoot;
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        CameraController.instance.FollowPlayer(transform);

        Vector2 movementVector = myPlayerControls.PlayerActions.Move.ReadValue<Vector2>();

        if (!movementVector.Equals(Vector2.zero))
        {
            Movement(movementVector);
            Rotate(movementVector);
        }

        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    [ServerRpc]
    private void ImpactDamageServerRpc(PlayerData player1, PlayerData player2)
    {
        Debug.Log("Impact Damage player1: " + player1.ImpactVelocity);
        Debug.Log("Impact Damage player2: " + player2.ImpactVelocity);
        if (player1.ImpactVelocity > 8)
        {
            Die();
        }
        else if (player1.ImpactVelocity > 6)
        {
            TakeDamage(2);
        }
        else if (player1.ImpactVelocity > 5)
        {
            TakeDamage(1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("NoImpactDamage"))
        {
            if (!IsOwner) return;
            if (collision.gameObject.TryGetComponent(out NetworkPlayerController networkPlayer))
            {
                var player1 = new PlayerData()
                {
                    Id = OwnerClientId,
                    ImpactVelocity = collision.relativeVelocity.magnitude
                };

                var player2 = new PlayerData()
                {
                    Id = networkPlayer.OwnerClientId,
                    ImpactVelocity = collision.relativeVelocity.magnitude
                };

                ImpactDamageServerRpc(player1, player2);
            }
        }
    }

    struct PlayerData : INetworkSerializable
    {
        public ulong Id;
        public float ImpactVelocity;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref ImpactVelocity);
        }
    }

    void Shoot(InputAction.CallbackContext context)
    {
        if (!IsOwner)
        {
            return;
        }
        myWeapon.ShootServerRpc();
    }

    void Movement(Vector2 movementVector)
    {
        myRigidbody2D.AddForce(movementVector * movementSpeed, ForceMode2D.Force);
    }

    void Rotate(Vector2 movementVector)
    {
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, movementVector);
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
        transform.rotation = newRotation;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth.Value > 0)
        {
            currentHealth.Value -= damage;
        }
        else
        {
            Die();
        }
        Debug.Log("You took " + damage + " damage! You have " + currentHealth.Value);
    }

    public void Die()
    {
        Debug.Log("You died");

        // Reset the position of the player to the start position
        myRigidbody2D.position = startPosition;
        myRigidbody2D.velocity = Vector2.zero;

        // Reset the health
        currentHealth.Value = maxHealth.Value;

        if (IsOwner)
        {
            DieServerRpc();
        }
    }

    [ServerRpc]
    public void DieServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("You are dead " + OwnerClientId);
    }
}
