using NetworkFunctionality;
using PlayerFunctionality;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeaponSystem;

namespace UserInterface
{
    /// <summary>
    /// The singleton class responsible for managing the UI elements, which work with specific weapon types. Currently not generic enough, needs some work later.
    /// </summary>
    public class NetworkPlayerUIManager : MonoBehaviour
    {
        [SerializeField] Button secondaryWeaponButton;
        [SerializeField] AmmoLeftText ammoLeftText;
        [SerializeField] TargetedEnemyTag targetedEnemyTag;
        [SerializeField] ChargeStatusBar chargeStatusBar;

        [SerializeField] GameObject[] elementsRequiringPlayerReference;

        [SerializeField] List<GameObject> activatedElements = new List<GameObject>();

        [SerializeField] GameObject referencedPlayerCharacter;

        private void FixedUpdate()
        {
            // Checking if the player was already assigned and if it can be
            if (referencedPlayerCharacter == null)
            {
                if (MultiplayerGameManager.instance.ownedPlayerCharacter.gameObject != null)
                {
                    AddReferences();
                }
            }
        }

        /// <summary>
        /// Method adding all required references to player character, delegates.
        /// </summary>
        void AddReferences()
        {
            // Assigning the player character owned by local player to the reference
            referencedPlayerCharacter = MultiplayerGameManager.instance.ownedPlayerCharacter.gameObject;

            // Adding the method to the delegate
            referencedPlayerCharacter.GetComponent<NetworkPlayerController>().onWeaponChanged += UpdateVisibility;

            // Assigning the local player character to all UI elements, which requires it
            ammoLeftText.networkPlayerCharacter = referencedPlayerCharacter;
            targetedEnemyTag.networkPlayerCharacter = referencedPlayerCharacter;
            chargeStatusBar.networkPlayerCharacter = referencedPlayerCharacter;

            foreach(GameObject element in elementsRequiringPlayerReference)
            {
                element.GetComponent<IRequirePlayerReference>().AddReferences(referencedPlayerCharacter);
            }
        }

        /// <summary>
        /// Method updating the visibility of all referenced UI elements.
        /// Assigned to onWeaponChanged event of NetworkPlayerCharacter.
        /// </summary>
        /// <param name="weaponClass">Class of weapon, which was equipped by the player</param>
        private void UpdateVisibility(WeaponClass weaponClass)
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

                ammoLeftText.gameObject.SetActive(false);
                secondaryWeaponButton.gameObject.SetActive(false);
                
                return;
            }

            // Activating the ammo left element, second shooting button and other depending on equipped gun
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
