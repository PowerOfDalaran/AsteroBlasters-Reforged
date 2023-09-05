using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerFunctionality
{
    public class MissileLauncher : Weapon
    {
        int maxAmmo;
        int currentAmmo;

        [SerializeField] float scanningPositionOffsetY = 9f;

        [SerializeField] float scanningSizeX = 13f;
        [SerializeField] float scanningSizeY = 14f;

        public delegate void OnTargetSwitch(Transform targetTransform);
        public static OnTargetSwitch onTargetSwitch;

        GameObject targetedEnemy;

        public MissileLauncher()
        {
            // Assigning the values to the properties
            type = WeaponType.ProjectileBased;
            fireCooldown = 2f;

            maxAmmo = 3;
            currentAmmo = maxAmmo;
        }

        private void FixedUpdate()
        {
            // Checking if there's any ammo left
            //if (currentAmmo <= 0) 
            //{
            //    PlasmaCannon plasmaCannon = GetComponent<PlasmaCannon>();
            //    DiscardWeapon(plasmaCannon);
            //}

            // Targeting system
            Vector2 scanningPositions = new Vector2(transform.position.x, transform.position.y + scanningPositionOffsetY);
            Vector2 scanningSize = new Vector2(scanningSizeX, scanningSizeY);

            Collider2D[] detectedColliders = Physics2D.OverlapBoxAll(scanningPositions, scanningSize, 0);
            foreach (Collider2D collider2D in detectedColliders)
            {

                IHealthSystem healthSystem = collider2D.gameObject.GetComponent<IHealthSystem>();
                if (healthSystem != null)
                {
                    if(targetedEnemy != collider2D.gameObject)
                    {
                        targetedEnemy = collider2D.gameObject;
                        onTargetSwitch?.Invoke(targetedEnemy.transform);
                    }
                    break;
                }
            }

            if (detectedColliders.Length == 0)
            {
                targetedEnemy = null;
                onTargetSwitch?.Invoke(null);
            }
        }

        public override GameObject Shoot()
        {
            // Checking if player can shoot
            if (currentAmmo > 0)
            {
                GameObject newMissile =  base.Shoot();
                Debug.Log("Broñ: " + targetedEnemy);
                newMissile.GetComponent<HomingMissile>().target = targetedEnemy.transform;

                currentAmmo -= 1;

                return newMissile;
            }

            return null;
        }

        /// <summary>
        /// Method activating removing this script and activating given weapon 
        /// </summary>
        /// <param name="weaponToActivate">Script of weapon, player switches to</param>
        public void DiscardWeapon(Weapon weaponToActivate)
        {
            weaponToActivate.enabled = true;

            Destroy(this);
        }
    }
}
