using TMPro;
using UnityEngine;
using WeaponSystem;

namespace UserInterface
{
    /// <summary>
    /// Class managing the ammo left UI element - basically updates it's text parameter
    /// </summary>
    public class AmmoLeftText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;

        private void OnEnable()
        {
            // Adding the method to the delegates of ALL weapons currently using the ammunition functionality
            // Need to generalize it later
            LaserSniperGun.onAmmoValueChange += UpdateText;
            MissileLauncher.onAmmoValueChange += UpdateText;
            PlasmaCannon.onAmmoValueChange += UpdateText;
        }

        private void OnDisable()
        {
            // Removing the method from the delegates of ALL weapons currently using the ammunition functionality
            // Need to generalize it later
            LaserSniperGun.onAmmoValueChange -= UpdateText;
            MissileLauncher.onAmmoValueChange -= UpdateText;
            PlasmaCannon.onAmmoValueChange -= UpdateText;
        }

        /// <summary>
        /// Updating the <c>text</c> property of the UI element, to represent the current amount of ammunition/maximum amount of ammunition
        /// </summary>
        /// <param name="currentValue">Current amount of possesed ammunition</param>
        /// <param name="maxValue">Maximum possible amount of ammunition</param>
        void UpdateText(int currentValue, int maxValue)
        {
            text.text = currentValue.ToString() + "/" + maxValue.ToString();
        }
    }
}