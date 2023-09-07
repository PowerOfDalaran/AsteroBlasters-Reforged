namespace WeaponSystem
{
    public class SpaceBullet : ProjectileController
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

