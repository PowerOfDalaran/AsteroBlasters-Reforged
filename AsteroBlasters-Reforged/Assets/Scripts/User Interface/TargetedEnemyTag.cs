using PlayerFunctionality;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class TargetedEnemyTag : MonoBehaviour
    {
        Transform currentTarget;

        void Start()
        {
            gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            MissileLauncher.onTargetSwitch += SwitchTarget;
        }

        void Update()
        {
            if (currentTarget == null) 
            {
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            }
            else
            {
                gameObject.transform.position =  RectTransformUtility.WorldToScreenPoint(Camera.main, currentTarget.position);
            }
        }

        void SwitchTarget(Transform newTarget)
        {
            currentTarget = newTarget;
            gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
        }
    }
}
