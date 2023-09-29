using PlayerFunctionality;
using UnityEngine;
using UnityEngine.UI;
using WeaponSystem;

namespace UserInterface
{
    /// <summary>
    /// Class managing the UI slider, displaying the heat level of plasma cannon (weapon)
    /// </summary>
    public class HeatWeaponBar : MonoBehaviour, IRequirePlayerReference
    {
        [SerializeField] Slider slider;

        /// <summary>
        /// Reference to player character game object. If this parameter isn't null, then the game is working in singleplayer mode.
        /// </summary>
        PlayerController playerCharacter;
        /// <summary>
        /// Reference to network player character game object. If this parameter isn't null, then the game is working in multiplayer mode.
        /// </summary>
        NetworkPlayerController networkPlayerCharacter;

        public void AddReferences(GameObject givenCharacter)
        {
            playerCharacter = givenCharacter.GetComponent<PlayerController>();
            networkPlayerCharacter = givenCharacter.GetComponent<NetworkPlayerController>();

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            // Subscribing to the events
            if (playerCharacter != null)
            {
                playerCharacter.GetComponent<SpaceRifle>().onHeatChanged += UpdateHeatbar;
            }

            if (networkPlayerCharacter != null)
            {
                networkPlayerCharacter.GetComponent<NetworkSpaceRifle>().onHeatChanged += UpdateHeatbar;
            }
        }

        private void OnDestroy()
        {
            // Removing the "UpdateHeatbar" method from the events
            if (playerCharacter != null)
            {
                playerCharacter.GetComponent<SpaceRifle>().onHeatChanged -= UpdateHeatbar;
            }

            if (networkPlayerCharacter != null)
            {
                networkPlayerCharacter.GetComponent<NetworkSpaceRifle>().onHeatChanged -= UpdateHeatbar;
            }
        }

        /// <summary>
        /// Method adjusting the slider to proper value
        /// </summary>
        /// <param name="heat">Value, which slider should display</param>
        void UpdateHeatbar(float heat)
        {
            slider.value = heat;
        }
    }
}
