using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerFunctionality
{
    /// <summary>
    /// Class managing the functionalities of homing missile projectile
    /// </summary>
    public class HomingMissile : ProjectileController
    {
        public Transform target;

        //public Transform Target { 
        //    get { return target; } 
        //    set { target = Target; } 
        //}

        bool lostTarget;

        protected override void Awake()
        {
            base.Awake();

            // Assigning the values to the properties
            speed = 15f;
            damage = 2;

            lostTarget = false;
        }

        private void FixedUpdate()
        {
            // Checking if target should still be pursued or the missile should just be launched ahead
            if (target != null)
            {
                myRigidbody2D.AddForce(target.position * speed);
            }
            else if (!lostTarget)
            {
                lostTarget = true;
                base.Launch();
            }
        }

        public override void Launch()
        {
            // Ignoring the Launch method to prevent missile from being fired at spawn
        }
    }
}
