using UnityEngine;

/// <summary>
/// Class managing the projectile's functionalities.
/// </summary>
public class ProjectileController : MonoBehaviour
{
    Rigidbody2D myRigidbody2D;
    public float speed = 20f;
    public int damage = 1;

    void Awake()
    {
        // Assigning values to class properties
        myRigidbody2D = GetComponent<Rigidbody2D>();

        // Firing the projectile forward
        myRigidbody2D.velocity = transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IHealthSystem healthSystem = collision.gameObject.GetComponent<IHealthSystem>();

        if (healthSystem != null)
        {
            healthSystem.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
