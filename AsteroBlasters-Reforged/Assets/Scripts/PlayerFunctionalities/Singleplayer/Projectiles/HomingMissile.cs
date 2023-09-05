using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerFunctionality
{
    public class HomingMissile : ProjectileController
    {
        public Transform target;

        public Transform Target { 
            get { return target; } 
            set { target = Target; } 
        }

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
            // Ignore
        }
    }
}
