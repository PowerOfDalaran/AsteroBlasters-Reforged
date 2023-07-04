using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Parent class for all "SceneButtonHandlers" type scripts
/// </summary>
public class SceneButtonHandler : MonoBehaviour
{
    [SerializeField]
    protected Button[] buttonsToDisable;

    /// <summary>
    /// Method enabling or disabling all buttons from "buttonsToDisable" array
    /// </summary>
    /// <param name="state">State you want to switch</param>
    protected void ChangeButtonsState(bool state)
    {
        foreach (Button button in buttonsToDisable)
        {
            button.enabled = state;
        }
    }
}
