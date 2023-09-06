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
        /// Temporary solution for the problem with passing the value of raycast length to children classes
        /// </summary>
        protected float raycastDistance;

        /// <summary>
        /// Method creating new projectile with certain position and rotation, if fire cooldown has passed.
        /// If weapon is Raycast based, the method fires the raycast in upwards and returns the gameobject, which one of components implement the IHealthSystem interface
        /// </summary>
        /// <param name="charge">The float value (0 - 10) representing how long the fire button was pressed</param>
        /// <returns>Created projectile (null if the weapon is on cooldown)</returns>
        public virtual GameObject Shoot(float charge)
        {
            if (Time.time > cooldownStatus)
            {
                cooldownStatus = Time.time + fireCooldown;

                if (type  == WeaponType.ProjectileBased)
                {
                    // Creating the projectile and returning the created object 
                    GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                    newProjectile.GetComponent<ProjectileController>().Launch();

                    return newProjectile;
                }
                else if(type == WeaponType.RaycastBased) 
                {
                    // Casting the Raycast and returning the hit object
                    RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.up);
                    if (hitInfo)
                    {
                        raycastDistance = hitInfo.distance;
                        return hitInfo.transform.gameObject;
                    }
                }
            }
            return null;
        }
    }
}
