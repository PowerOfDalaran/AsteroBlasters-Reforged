using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Class responsible for firing projectiles from certain position.
/// </summary>
public class Weapon : NetworkBehaviour
{
    [SerializeField]
    Transform firePoint;
    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    float fireCooldown = 1;
    float cooldownStatus = 0;

    /// <summary>
    /// Method creating new projectile with certain position and rotation, if fire cooldown has passed.
    /// </summary>
    [ServerRpc]
    public void ShootServerRpc()
    {
        if (Time.time > cooldownStatus)
        {
            cooldownStatus = Time.time + fireCooldown;
            GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            newProjectile.GetComponent<NetworkObject>().Spawn();
            newProjectile.GetComponent<ProjectileController>().Launch();
        }
    }
}
