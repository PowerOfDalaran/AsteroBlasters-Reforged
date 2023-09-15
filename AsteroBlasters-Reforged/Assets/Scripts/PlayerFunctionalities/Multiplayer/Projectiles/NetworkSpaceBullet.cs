namespace WeaponSystem
{
    /// <summary>
    /// Class controlling the <c>SpaceBullet</c> projectile (network version)
    /// </summary>
    public class NetworkSpaceBullet : NetworkProjectileController
    {
        protected override void Awake()
        {
            base.Awake();

            // Assigning the values to the properties
            speed = 20;
            damage = 1;
        }
    }
}
