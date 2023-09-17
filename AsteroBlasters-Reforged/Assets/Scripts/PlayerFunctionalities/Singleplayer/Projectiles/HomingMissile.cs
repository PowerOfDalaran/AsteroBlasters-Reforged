using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class managing the functionalities of homing missile projectile
    /// </summary>
    public class HomingMissile : ProjectileController
    {
        Transform target;

        public Transform Target
        {
            get { return target; }
            set { target = value; }
        }

        float homingSpeed;
        float rotationSpeed;

        bool lostTarget;

        protected override void Awake()
        {
            base.Awake();

            // Assigning the values to the properties
            speed = 15f;
            damage = 2;

            rotationSpeed = 360f;
            homingSpeed = 6.4f;
            lostTarget = false;
        }

        private void FixedUpdate()
        {
            // Checking if target should still be pursued or the missile should just be launched ahead
            if (target != null)
            {
                Vector2 direction = (Vector2)target.position - myRigidbody2D.position;

                direction.Normalize();

                float rotateAmount = Vector3.Cross(direction, transform.up).z;

                myRigidbody2D.angularVelocity = -rotateAmount * rotationSpeed;

                myRigidbody2D.velocity = transform.up * homingSpeed;
            }
            else if (!lostTarget)
            {
                lostTarget = true;
                base.Launch();
            }
        }
    }
}
