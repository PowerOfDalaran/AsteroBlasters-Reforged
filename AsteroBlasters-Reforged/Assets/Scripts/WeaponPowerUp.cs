using PlayerFunctionality;
using UnityEngine;
using WeaponSystem;

public class WeaponPowerUp : PowerUp
{
    [SerializeField] WeaponClass grantedWeapon;
    [SerializeField] GameObject projectilePrefab;

    protected override void BuffPlayer(PlayerController playerController)
    {
        playerController.PickNewSecondaryWeapon(grantedWeapon, projectilePrefab);
        Destroy(gameObject);
    }
}
