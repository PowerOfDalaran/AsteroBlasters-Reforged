namespace WeaponSystem
{
    /// <summary>
    /// Class providing the functionality for base player weapon (network version).
    /// </summary>
    public class NetworkSpaceRifle : NetworkWeapon
    {
        bool overheated;

        float currentHeat;
        float maxHeat;

        float heatLoss;
        float heatGain;

        public delegate void OnHeatChanged(float heat);
        public event OnHeatChanged onHeatChanged;

        void Start()
        {
            // Instantiating the wepon parameters on start, since this weapon is always active and available for player
            InstantiateWeapon();
        }

        private void FixedUpdate()
        {
            // Checking wether the weapon is overheated, or should be overheated, or isn't overheated
            if (overheated)
            {
                // Decreasing current heat and running the event
                currentHeat -= heatLoss;
                onHeatChanged?.Invoke(currentHeat);

                // Checking if weapon should still be overheated 
                if (currentHeat <= 0f)
                {
                    overheated = false;
                }
            }
            else if (currentHeat >= maxHeat)
            {
                // Turning overheated state to true
                overheated = true;
            }
            else if (currentHeat > 0)
            {
                // Decreasing current heat and running the event
                currentHeat -= heatLoss;
                onHeatChanged?.Invoke(currentHeat);
            }
        }

        public override void InstantiateWeapon()
        {
            type = WeaponType.ProjectileBased;
            weaponClass = WeaponClass.SpaceRifle;
            fireCooldown = 0.125f;

            overheated = false;
            currentHeat = 0f;
            maxHeat = 1f;

            heatLoss = 0.0085f;
            heatGain = 0.15f;
        }

        public override bool Shoot(float charge)
        {
            if (!overheated)
            {
                bool weaponFired = base.Shoot(charge);

                if (weaponFired)
                {
                    // If weapon actually fired, generating heat and launching the events
                    currentHeat += heatGain;
                    onHeatChanged?.Invoke(currentHeat);

                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
