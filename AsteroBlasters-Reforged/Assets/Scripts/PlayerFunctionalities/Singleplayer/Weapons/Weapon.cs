using UnityEngine;

namespace PlayerFunctionality
{
    public enum WeaponType
    {
        RaycastBased,
        ProjectileBased,
    }

    /// <summary>
    /// Class responsible for firing projectiles from certain position.
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [SerializeField] protected Transform firePoint;

        protected WeaponType type;

        [SerializeField] protected float fireCooldown = 1;
        protected float cooldownStatus = 0;

        public delegate void OnWeaponChanged(GameObject thisGameObject);
        public static event OnWeaponChanged onWeaponChanged;

        [SerializeField] protected GameObject projectilePrefab;

        /// <summary>
        /// Method creating new projectile with certain position and rotation, if fire cooldown has passed.
        /// If weapon is Raycast based, the method currently don't do anything.
        /// </summary>
        /// <returns>Created projectile (null if the weapon is on cooldown or raycast based)</returns>
        public virtual GameObject Shoot(float charge)
        {
            if (Time.time > cooldownStatus)
            {
                cooldownStatus = Time.time + fireCooldown;

                if (type  == WeaponType.ProjectileBased)
                {
                    GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                    newProjectile.GetComponent<ProjectileController>().Launch();

                    return newProjectile;
                }
                else if(type == WeaponType.RaycastBased) 
                {
                    RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.up);
                    if (hitInfo)
                    {
                        IHealthSystem healthSystem = hitInfo.transform.gameObject.GetComponent<IHealthSystem>();
                        if (healthSystem != null)
                        {
                            return hitInfo.transform.gameObject;
                        }
                    }
                }
            }
            return null;
        }
    }
}
