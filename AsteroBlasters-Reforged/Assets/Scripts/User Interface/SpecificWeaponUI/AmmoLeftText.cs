using TMPro;
using UnityEngine;
using WeaponSystem;

namespace UserInterface
{
    public class AmmoLeftText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;

        private void OnEnable()
        {
            LaserSniperGun.onAmmoValueChange += UpdateText;
            MissileLauncher.onAmmoValueChange += UpdateText;
        }

        private void OnDisable()
        {
            LaserSniperGun.onAmmoValueChange -= UpdateText;
            MissileLauncher.onAmmoValueChange -= UpdateText;
        }

        void UpdateText(int currentValue, int maxValue)
        {
            text.text = currentValue.ToString() + "/" + maxValue.ToString();
        }
    }
}
