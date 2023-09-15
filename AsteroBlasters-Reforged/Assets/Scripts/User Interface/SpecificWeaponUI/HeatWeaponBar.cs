using UnityEngine;
using UnityEngine.UI;
using WeaponSystem;

namespace UserInterface
{
    /// <summary>
    /// Class managing the UI slider, displaying the heat level of plasma cannon (weapon)
    /// </summary>
    public class HeatWeaponBar : MonoBehaviour
    {
        [SerializeField] Slider slider;

        /// <summary>
        /// Reference to player character game object. If this parameter isn't null, then the game is working in singleplayer mode.
        /// </summary>
        [SerializeField] public GameObject playerCharacter;
        /// <summary>
        /// Reference to network player character game object. If this parameter isn't null, then the game is working in multiplayer mode.
        /// </summary>
        [SerializeField] public GameObject networkPlayerCharacter;

        void OnEnable()
        {
            // Adding the "UpdateHeatbar" method to the events
            if (playerCharacter != null)
            {
                playerCharacter.GetComponent<PlasmaCannon>().onHeatChanged += UpdateHeatbar;
            }

            if (networkPlayerCharacter != null)
            {
                networkPlayerCharacter.GetComponent<NetworkPlasmaCannon>().onHeatChanged += UpdateHeatbar;
            }
        }

        private void OnDisable()
        {
            // Removing the "UpdateHeatbar" method from the events
            if (playerCharacter != null)
            {
                playerCharacter.GetComponent<PlasmaCannon>().onHeatChanged -= UpdateHeatbar;
            }

            if (networkPlayerCharacter != null)
            {
                networkPlayerCharacter.GetComponent<NetworkPlasmaCannon>().onHeatChanged -= UpdateHeatbar;
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
