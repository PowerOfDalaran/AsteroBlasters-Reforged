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
    SpriteRenderer mySpriteRenderer;

    [SerializeField]
    float movementSpeed = 3f;
    [SerializeField]
    float rotationSpeed = 720;
    public int playerIndex;

    public NetworkVariable<int> maxHealth = new NetworkVariable<int>();
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    //public NetworkVariable<float> impactVelocity = new NetworkVariable<float>();

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

    [ClientRpc]
    public void SetMyIndexClientRpc(int givenIndex)
    {
        playerIndex = givenIndex;
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

        // Checking if the player is still alive
        if (currentHealth.Value <= 0)
        {
            //Debug.Log(currentHealth);

        }
    }

    [ServerRpc]
    private void ImpactDamageServerRpc(PlayerInGameData player1, PlayerInGameData player2)
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

    public void TakeDamage(int damage)
    {
        TakeDamageServerRpc(damage);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<NetworkPlayerController>();

        if (client.currentHealth.Value > 0)
        {
            client.currentHealth.Value -= damage;
        }
        else
        {
            client.currentHealth.Value = 3;
            DieServerRpc();
        }
        Debug.Log("You took " + damage + " damage!, You have " + client.currentHealth.Value);
    }

    public void Die()
    {
        Debug.Log("You died");

        //DieServerRpc();

    }

    [ServerRpc]
    public void DieServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<NetworkPlayerController>();
        client.myRigidbody2D.position = new Vector2(0f, 0f);
        client.myRigidbody2D.velocity = Vector2.zero;
    }
}