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
        [SerializeField] public GameObject playerCharacter;
        [SerializeField] public GameObject networkPlayerCharacter;

        void OnEnable()
        {
            if (playerCharacter != null)
            {
                playerCharacter.GetComponent<PlayerController>().onChargeValueChanged += UpdateChargeBar;
            }

            if (networkPlayerCharacter != null)
            {
                playerCharacter.GetComponent<NetworkPlayerController>().onChargeValueChanged += UpdateChargeBar;
            }
        }

        private void OnDisable()
        {
            if (playerCharacter != null)
            {
                playerCharacter.GetComponent<PlayerController>().onChargeValueChanged -= UpdateChargeBar;
            }

            if (networkPlayerCharacter != null)
            {
                playerCharacter.GetComponent<NetworkPlayerController>().onChargeValueChanged -= UpdateChargeBar;
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
