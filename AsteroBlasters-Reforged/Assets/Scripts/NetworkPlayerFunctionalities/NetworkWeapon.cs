using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Class responsible for firing projectiles from certain position.
/// This version is also using multiple Netcode methods to allow playing in multiplayer mode.
/// </summary>
public class NetworkWeapon : NetworkBehaviour
{
    [SerializeField]
    Transform firePoint;
    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    float fireCooldown = 1;
    float cooldownStatus = 0;

    /// <summary>
    /// Method, which using ServerRpc, creates new projectile, spawn and launch it, if fire cooldown has passed.
    /// </summary>
    [ServerRpc]
    public void ShootServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong projectileOwner = serverRpcParams.Receive.SenderClientId;

        if (Time.time > cooldownStatus)
        {
            cooldownStatus = Time.time + fireCooldown;
            GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            newProjectile.GetComponent<NetworkObject>().SpawnWithOwnership(projectileOwner);
            newProjectile.GetComponent<NetworkProjectileController>().Launch();
        }
    }
}
