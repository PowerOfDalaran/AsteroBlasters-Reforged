namespace WeaponSystem
{
    /// <summary>
    /// Class providing the functionality for base player weapon.
    /// </summary>
    public class SpaceRifle : Weapon
    {
        private void Start()
        {
            // Instantiating the wepon parameters on start, since this weapon is always active and available for player
            InstantiateWeapon();
        }

        public override void InstantiateWeapon()
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            weaponClass = WeaponClass.SpaceRifle;
            fireCooldown = 0.35f;
        }
    }
}
