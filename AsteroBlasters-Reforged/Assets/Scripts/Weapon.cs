using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for firing projectiles from certain position.
/// </summary>
public class Weapon : MonoBehaviour
{
    [SerializeField]
    Transform firePoint;
    [SerializeField]
    GameObject projectilePrefab;

    /// <summary>
    /// Method creating new projectile with certain position and rotation.
    /// </summary>
    public void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
}
