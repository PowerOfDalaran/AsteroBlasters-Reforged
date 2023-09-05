using PlayerFunctionality;
using UnityEngine;
using UnityEngine.UI;

public class HeatWeaponBar : MonoBehaviour
{
    [SerializeField] Slider slider;

    void Start()
    {
        PlasmaCannon.onHeatChanged += UpdateHeatbar;
    }

    void UpdateHeatbar(float heat)
    {
        slider.value = heat;
    }
}
