using PlayerFunctionality;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class WeaponUIManager : MonoBehaviour
{
    [SerializeField] GameObject heatWeaponBar;
    [SerializeField] GameObject AmmoLeftText;
    [SerializeField] GameObject TargetedEnemyTag;
    [SerializeField] GameObject chargeStatusBar;

    List<GameObject> activatedElements = new List<GameObject>();

    void Start()
    {
        PlayerController.onWeaponChanged += UpdateVisibility;
    }

    private void OnDestroy()
    {
        PlayerController.onWeaponChanged -= UpdateVisibility;
    }

    void UpdateVisibility(WeaponClass weaponClass)
    {
        if (weaponClass == WeaponClass.None)
        {
            foreach (GameObject weapon in activatedElements) 
            { 
                weapon.SetActive(false);
                AmmoLeftText.SetActive(true);
                activatedElements.Remove(weapon);
            };
            return;
        }

        AmmoLeftText.SetActive(true);

        switch (weaponClass)
        {
            case WeaponClass.PlasmaCannon:
                heatWeaponBar.SetActive(true);
                activatedElements.Add(heatWeaponBar);
                break;
            case WeaponClass.MissileLauncher:
                TargetedEnemyTag.SetActive(true);
                activatedElements.Add(TargetedEnemyTag);
                break;
            case WeaponClass.LaserSniperGun:
                chargeStatusBar.SetActive(true);
                activatedElements.Add(chargeStatusBar);
                break;
            default:
                Debug.Log("Unexpected weapon class was given: " + weaponClass);
                break;
        }
    }
}
