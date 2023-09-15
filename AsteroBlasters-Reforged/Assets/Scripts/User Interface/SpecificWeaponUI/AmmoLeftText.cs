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

        /// <summary>
        /// Reference to player character game object. If this parameter isn't null, then the game is working in singleplayer mode.
        /// </summary>
        [SerializeField] public GameObject playerCharacter;
        /// <summary>
        /// Reference to network player character game object. If this parameter isn't null, then the game is working in multiplayer mode.
        /// </summary>
        [SerializeField] public GameObject networkPlayerCharacter;

        private void OnEnable()
        {
            // Adding the method to the delegates of ALL weapons currently using the ammunition functionality
            // Need to generalize it later
            if (playerCharacter != null)
            {
                playerCharacter.GetComponent<LaserSniperGun>().onAmmoValueChange += UpdateText;
                playerCharacter.GetComponent<MissileLauncher>().onAmmoValueChange += UpdateText;
                playerCharacter.GetComponent<PlasmaCannon>().onAmmoValueChange += UpdateText;
            }

            if (networkPlayerCharacter != null)
            {
                networkPlayerCharacter.GetComponent<NetworkLaserSniperGun>().onAmmoValueChange += UpdateText;
                networkPlayerCharacter.GetComponent<NetworkMissileLauncher>().onAmmoValueChange += UpdateText;
                networkPlayerCharacter.GetComponent<NetworkPlasmaCannon>().onAmmoValueChange += UpdateText;
            }
        }

        private void OnDisable()
        {
            // Removing the method from the delegates of ALL weapons currently using the ammunition functionality
            // Need to generalize it later
            if (playerCharacter != null)
            {
                playerCharacter.GetComponent<LaserSniperGun>().onAmmoValueChange -= UpdateText;
                playerCharacter.GetComponent<MissileLauncher>().onAmmoValueChange -= UpdateText;
                playerCharacter.GetComponent<PlasmaCannon>().onAmmoValueChange -= UpdateText;
            }

            if (networkPlayerCharacter != null)
            {
                networkPlayerCharacter.GetComponent<NetworkLaserSniperGun>().onAmmoValueChange -= UpdateText;
                networkPlayerCharacter.GetComponent<NetworkMissileLauncher>().onAmmoValueChange -= UpdateText;
                networkPlayerCharacter.GetComponent<NetworkPlasmaCannon>().onAmmoValueChange -= UpdateText;
            }
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
