using PlayerFunctionality;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    /// <summary>
    /// Class managing the UI slider, displaying the charged power of current shot 
    /// </summary>
    public class ChargeStatusBar : MonoBehaviour
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
            // Adding the "UpdateChargeBar" method to the events
            if (playerCharacter != null)
            {
                playerCharacter.GetComponent<PlayerController>().onChargeValueChanged += UpdateChargeBar;
            }

            if (networkPlayerCharacter != null)
            {
                networkPlayerCharacter.GetComponent<NetworkPlayerController>().onChargeValueChanged += UpdateChargeBar;
            }
        }

        private void OnDisable()
        {
            // Removing the "UpdateChargeBar" method from the events
            if (playerCharacter != null)
            {
                playerCharacter.GetComponent<PlayerController>().onChargeValueChanged -= UpdateChargeBar;
            }

            if (networkPlayerCharacter != null)
            {
                networkPlayerCharacter.GetComponent<NetworkPlayerController>().onChargeValueChanged -= UpdateChargeBar;
            }
        }

        /// <summary>
        /// Method adjusting the slider to proper value
        /// </summary>
        /// <param name="value">Value, which slider should display</param>
        void UpdateChargeBar(float value)
        {
            slider.value = value;
        }
    }
}
