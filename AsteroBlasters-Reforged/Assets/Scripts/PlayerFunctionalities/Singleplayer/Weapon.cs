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

        [SerializeField] protected GameObject projectilePrefab;

        /// <summary>
        /// Method creating new projectile with certain position and rotation, if fire cooldown has passed.
        /// </summary>
        public virtual void Shoot()
        {
            if (Time.time > cooldownStatus)
            {
                cooldownStatus = Time.time + fireCooldown;

                if (type  == WeaponType.ProjectileBased)
                {
                    GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                    newProjectile.GetComponent<ProjectileController>().Launch();
                }
                else if(type == WeaponType.RaycastBased) 
                {
                    // Implement in child classes
                }
            }
        }
    }
}
