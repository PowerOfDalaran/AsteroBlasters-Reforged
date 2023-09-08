using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Enumerator representing all the classes of weapon, which can be equipped (None representing the weapon being removed)
    /// </summary>
    public enum WeaponClass
    {
        None,
        SpaceRifle,
        PlasmaCannon,
        MissileLauncher,
        LaserSniperGun,
    }

    /// <summary>
    /// Enumerator representing whether the weapon deals with shooting by creating the projectile or by firing the raycast
    /// </summary>
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
        protected Transform firePoint;

        protected WeaponType type;
        protected WeaponClass weaponClass;

        [SerializeField] protected float fireCooldown = 1;
        protected float cooldownStatus = 0;

        [SerializeField] protected GameObject projectilePrefab;

        /// <summary>
        /// Temporary solution for the problem with passing the value of raycast length to children classes
        /// </summary>
        protected float raycastDistance;

        private void Awake()
        {
            firePoint = gameObject.transform.Find("FirePoint");
        }

        public virtual void InstantiateWeapon(GameObject projectile = null)
        { 
            // Implement in child classes
        }

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
