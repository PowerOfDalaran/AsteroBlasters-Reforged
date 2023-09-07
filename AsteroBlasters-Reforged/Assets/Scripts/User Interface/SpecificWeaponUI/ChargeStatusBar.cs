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

        void Start()
        {
            PlayerController.onChargeValueChanged += UpdateChargeBar;
        }

        private void OnDestroy()
        {
            PlayerController.onChargeValueChanged -= UpdateChargeBar;
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
