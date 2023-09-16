using PlayerFunctionality;
using System;
using Unity.Netcode;
using UnityEngine;
using WeaponSystem;

namespace PickableObjects
{
    /// <summary>
    /// Class managing the power ups granting the new weapon for the player
    /// </summary>
    public class NetworkWeaponPowerUp : NetworkPowerUp
    {
        [NonSerialized] public NetworkVariable<WeaponClass> grantedWeapon;
         public WeaponClass GrantedWeapon;

        private void Awake()
        {
            grantedWeapon = new NetworkVariable<WeaponClass>();
        }

        protected override void Start()
        {
            base.Start();
            grantedWeapon.Value = GrantedWeapon;
        }

        protected override void BuffPlayer(NetworkPlayerController playerController)
        {
            playerController.PickNewSecondaryWeapon(grantedWeapon.Value);
        }
    }
}
