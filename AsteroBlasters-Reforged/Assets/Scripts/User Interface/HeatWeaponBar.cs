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

        void Start()
        {
            PlasmaCannon.onHeatChanged += UpdateHeatbar;
        }

        private void OnDestroy()
        {
            PlasmaCannon.onHeatChanged -= UpdateHeatbar;
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
