using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneButtonHandler : MonoBehaviour
{
    [SerializeField]
    protected Button[] buttonsToDisable;

    protected void ChangeButtonsState(bool state)
    {
        foreach (Button button in buttonsToDisable)
        {
            button.enabled = state;
        }
    }
}
