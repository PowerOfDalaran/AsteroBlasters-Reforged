using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour, IHealthSystem
{
    Weapon myWeapon;

    [SerializeField]
    int maxHealth = 2;
    [SerializeField]
    int currentHealth;

    public void Die()
    {
        Debug.Log("Dummy: I die!");
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Dummy: I take " + damage + " point of damage!");
        currentHealth -= damage;
    }

    void Awake()
    {
        myWeapon = gameObject.GetComponent<Weapon>();
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        // Checking if dummy is still alive
        if (currentHealth <= 0) 
        {
            Die();
        }

        if (myWeapon != null)
        {
            myWeapon.Shoot();
        }
    }
}
