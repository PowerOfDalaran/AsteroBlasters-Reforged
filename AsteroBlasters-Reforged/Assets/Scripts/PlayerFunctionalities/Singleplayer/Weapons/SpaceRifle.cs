namespace PlayerFunctionality
{
    public class SpaceRifle : Weapon
    {
        public SpaceRifle()
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            fireCooldown = 0.35f;
        }
    }
}
