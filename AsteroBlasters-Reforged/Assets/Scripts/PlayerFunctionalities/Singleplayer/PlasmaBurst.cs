namespace PlayerFunctionality
{
    /// <summary>
    /// Child class responsible for managing the Plasma Burst projectiles
    /// </summary>
    public class PlasmaBurst : ProjectileController
    {
        protected override void Awake()
        {
            base.Awake();

            // Assigning the values to the properties
            speed = 25f;
            damage = 1;
        }
    }
}
