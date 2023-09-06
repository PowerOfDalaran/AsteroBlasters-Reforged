using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerFunctionality
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

        float rotationSpeed;
        float homingSpeed;
        bool lostTarget;

        protected override void Awake()
        {
            base.Awake();

            // Assigning the values to the properties
            speed = 8f;
            damage = 2;

            rotationSpeed = 10f;
            homingSpeed = 5f;
            lostTarget = false;
        }

        private void FixedUpdate()
        {
            // Checking if target should still be pursued or the missile should just be launched ahead
            if (target != null)
            {
                myRigidbody2D.AddForce((target.position - transform.position) * homingSpeed);
            }
            else if (!lostTarget)
            {
                lostTarget = true;
                base.Launch();
            }

            // Rotating the missile
            Rotate();
        }

        /// <summary>
        /// Method rotating the game object in direction its flying towards
        /// </summary>
        private void Rotate()
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, myRigidbody2D.velocity);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
            gameObject.transform.rotation = newRotation;
        }
    }
}
