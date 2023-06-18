using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class managing the projectile's functionalities.
/// </summary>
public class ProjectileController : MonoBehaviour
{
    Rigidbody2D myRigidbody2D;
    public float speed = 20f;

    void Awake()
    {
        // Assigning values to class properties
        myRigidbody2D = GetComponent<Rigidbody2D>();

        // Firing the projectile forward
        myRigidbody2D.velocity = transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Currently simply destroying the projectile.
        // More functionality will be added with creation of healthpoints system.
        Destroy(gameObject);
    }
}
