using PlayerFunctionality;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeaponSystem;

namespace UserInterface
{
    /// <summary>
    /// Class changing the visibilty of all UI elements related to player character
    /// </summary>
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] Button secondaryWeaponButton;
        [SerializeField] AmmoLeftText ammoLeftText;
        [SerializeField] TargetedEnemyTag targetedEnemyTag;
        [SerializeField] ChargeStatusBar chargeStatusBar;

        [SerializeField] GameObject[] elementsRequiringPlayerReference;

        [SerializeField] List<GameObject> activatedElements = new List<GameObject>();

        [SerializeField] GameObject playerCharacter;

        void Start()
        {
            playerCharacter.GetComponent<PlayerController>().onWeaponChanged += UpdateVisibility;

            ammoLeftText.playerCharacter = playerCharacter;
            targetedEnemyTag.playerCharacter = playerCharacter;
            chargeStatusBar.playerCharacter = playerCharacter;

            foreach (GameObject element in elementsRequiringPlayerReference) 
            {
                element.GetComponent<IRequirePlayerReference>().AddReferences(playerCharacter);
            }
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
                    activatedElements.Remove(currentWeaponUI);
                }

                // Turning off the elements thata are shared by every secondary weapon
                ammoLeftText.gameObject.SetActive(false);
                secondaryWeaponButton.gameObject.SetActive(false);

                return;
            }

            // Activating the ammo left element and other depending on equipped gun
            ammoLeftText.gameObject.SetActive(true);
            secondaryWeaponButton.gameObject.SetActive(true);


            switch (weaponClass)
            {
                case WeaponClass.PlasmaCannon:
                    break;
                case WeaponClass.MissileLauncher:
                    targetedEnemyTag.gameObject.SetActive(true);
                    activatedElements.Add(targetedEnemyTag.gameObject);
                    break;
                case WeaponClass.LaserSniperGun:
                    chargeStatusBar.gameObject.SetActive(true);
                    activatedElements.Add(chargeStatusBar.gameObject);
                    break;
                default:
                    Debug.Log("Unexpected weapon class was given: " + weaponClass);
                    break;
            }
        }
    }
}
