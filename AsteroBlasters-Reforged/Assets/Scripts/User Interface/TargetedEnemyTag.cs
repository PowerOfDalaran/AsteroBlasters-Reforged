using PlayerFunctionality;
using UnityEngine;
using UnityEngine.UI;
using WeaponSystem;

namespace UserInterface
{
    /// <summary>
    /// Class managing the tag, which appears on the enemy currently targeted by Missile Launcher weapon
    /// </summary>
    public class TargetedEnemyTag : MonoBehaviour
    {
        Transform currentTarget;

        void Start()
        {
            // Turning the visibility of tag off and adding the update method to the delegate
            gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);
        }

        private void OnEnable()
        {
            MissileLauncher.onTargetSwitch += SwitchTarget;
        }

        private void OnDisable()
        {
            MissileLauncher.onTargetSwitch -= SwitchTarget;
        }

        void Update()
        {
            // Checking if tag should turn off or set its position of target
            if (currentTarget == null) 
            {
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            }
            else
            {
                gameObject.transform.position =  RectTransformUtility.WorldToScreenPoint(Camera.main, currentTarget.position);
            }
        }

        /// <summary>
        /// Method changing the followed target to the new one and setting the visibilty on
        /// </summary>
        /// <param name="newTarget">Transform of new target to follow</param>
        void SwitchTarget(Transform newTarget)
        {
            currentTarget = newTarget;
            gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
        }
    }
}
