namespace WeaponSystem
{
    public class SpaceRifle : Weapon
    {
        public SpaceRifle()
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            weaponClass = WeaponClass.SpaceRifle;
            fireCooldown = 0.35f;
        }
    }
}
