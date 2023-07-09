using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Class managing the projectile's functionalities.
/// This version is also using multiple Netcode methods to allow playing in multiplayer mode.
/// </summary>
public class NetworkProjectileController : NetworkBehaviour
{
    Rigidbody2D myRigidbody2D;
    public float speed = 20f;
    public int damage = 1;

    void Awake()
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

        DespawnSelfServerRpc();
    }

    /// <summary>
    /// Method granting the prefab velocity, to launch it in current direction
    /// </summary>
    public void Launch()
    {
        myRigidbody2D.velocity = transform.up * speed;
    }

    /// <summary>
    /// Method calling the host to despawn this projectile
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    void DespawnSelfServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
