using PlayerFunctionality;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
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

        void UpdateChargeBar(float value)
        {
            slider.value = value;
        }
    }
}
