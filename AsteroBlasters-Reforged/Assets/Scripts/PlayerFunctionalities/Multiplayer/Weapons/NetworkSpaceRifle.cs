namespace WeaponSystem
{
    /// <summary>
    /// Class providing the functionality for base player weapon (network version).
    /// </summary>
    public class NetworkSpaceRifle : NetworkWeapon
    {
        void Start()
        {
            // Instantiating the wepon parameters on start, since this weapon is always active and available for player
            InstantiateWeapon();
        }

        public override void InstantiateWeapon()
        {
            type = WeaponType.ProjectileBased;
            weaponClass = WeaponClass.SpaceRifle;
            fireCooldown = 0.35f;
        }
    }
}
