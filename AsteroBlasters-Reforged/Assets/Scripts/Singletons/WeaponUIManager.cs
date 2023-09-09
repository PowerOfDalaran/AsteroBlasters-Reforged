using PlayerFunctionality;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

namespace UserInterface
{
    /// <summary>
    /// Class changing the visibilty of all UI elements assigned to specific type of weapon
    /// </summary>
    public class WeaponUIManager : MonoBehaviour
    {
        [SerializeField] GameObject secondaryWeaponButton;
        [SerializeField] GameObject heatWeaponBar;
        [SerializeField] GameObject AmmoLeftText;
        [SerializeField] GameObject TargetedEnemyTag;
        [SerializeField] GameObject chargeStatusBar;

        [SerializeField] List<GameObject> activatedElements = new List<GameObject>();

        void Start()
        {
            PlayerController.onWeaponChanged += UpdateVisibility;
        }

        private void OnDestroy()
        {
            PlayerController.onWeaponChanged -= UpdateVisibility;
        }

        /// <summary>
        /// Method updating the visibilty of all UI elements depending on secondary weapon equipped by player
        /// </summary>
        /// <param name="weaponClass">Special enumerator representing the weapon class</param>
        void UpdateVisibility(WeaponClass weaponClass)
        {
            // Checking if the weapon wasn't unequipped - then all the UI elements should be disabled
            if (weaponClass == WeaponClass.None)
            {
                for (int i = 0; i < activatedElements.Count; i++)
                {
                    GameObject currentWeaponUI = activatedElements[i];

                    currentWeaponUI.SetActive(false);
                    AmmoLeftText.SetActive(false);
                    secondaryWeaponButton.SetActive(false);
                    activatedElements.Remove(currentWeaponUI);
                }

                return;
            }

            // Activating the ammo left element and other depending on equipped gun
            AmmoLeftText.SetActive(true);
            secondaryWeaponButton.SetActive(true);


            switch (weaponClass)
            {
                case WeaponClass.PlasmaCannon:
                    heatWeaponBar.SetActive(true);
                    activatedElements.Add(heatWeaponBar);
                    break;
                case WeaponClass.MissileLauncher:
                    TargetedEnemyTag.SetActive(true);
                    activatedElements.Add(TargetedEnemyTag);
                    break;
                case WeaponClass.LaserSniperGun:
                    chargeStatusBar.SetActive(true);
                    activatedElements.Add(chargeStatusBar);
                    break;
                default:
                    Debug.Log("Unexpected weapon class was given: " + weaponClass);
                    break;
            }
        }
    }
}
