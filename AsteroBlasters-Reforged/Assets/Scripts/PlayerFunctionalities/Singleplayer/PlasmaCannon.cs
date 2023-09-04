using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerFunctionality
{
    public class PlasmaCannon : Weapon
    {
        [SerializeField]  bool overheated;

        [SerializeField] float currentHeat;
        [SerializeField] float maxHeat;

        public PlasmaCannon() 
        {
            type = WeaponType.ProjectileBased;
            fireCooldown = 0.2f;

            overheated = false;
            currentHeat = 0f;
            maxHeat = 1f;
        }

        private void FixedUpdate()
        {
            if (overheated)
            {
                currentHeat -= 0.01f;

                if (currentHeat <= 0f) 
                {
                    overheated = false;
                }
            }
            else if (currentHeat >= maxHeat) 
            {
                overheated = true;
            }
            else if (currentHeat > 0)
            {
                currentHeat -= 0.005f;
            }
        }

        public override void Shoot()
        {
            if (!overheated)
            {
                base.Shoot();
                currentHeat += 0.125f;
            }
            else
            {
                Debug.Log("Przegrzaaanieeee!");
            }
        }
    }
}
