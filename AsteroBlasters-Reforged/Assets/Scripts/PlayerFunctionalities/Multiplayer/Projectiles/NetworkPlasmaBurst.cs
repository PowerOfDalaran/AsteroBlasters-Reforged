namespace WeaponSystem
{
    public class NetworkPlasmaBurst : NetworkProjectileController
    {
        protected override void Awake()
        {
            base.Awake();

            // Assigning the values to the properties
            speed = 17.5f;
            damage = 2;
        }
    }
}
