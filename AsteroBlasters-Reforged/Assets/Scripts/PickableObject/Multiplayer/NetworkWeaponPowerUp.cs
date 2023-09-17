using PlayerFunctionality;
using System;
using Unity.Netcode;
using WeaponSystem;

namespace PickableObjects
{
    /// <summary>
    /// Class managing the power ups granting the new weapon for the player
    /// </summary>
    public class NetworkWeaponPowerUp : NetworkPowerUp
    {
        [NonSerialized] NetworkVariable<WeaponClass> grantedWeapon;
        /// <summary>
        /// Special property, which value will be assigned to the network variable on start of the script.
        /// </summary>
        public WeaponClass GrantedWeapon;

        private void Awake()
        {
            // Instantiating the network variable
            grantedWeapon = new NetworkVariable<WeaponClass>();
        }

        protected override void Start()
        {
            base.Start();

            if (IsHost)
            {
                grantedWeapon.Value = GrantedWeapon;
            }
        }

        protected override void BuffPlayer(NetworkPlayerController playerController)
        {
            playerController.PickNewSecondaryWeapon(grantedWeapon.Value);
        }
    }
}
