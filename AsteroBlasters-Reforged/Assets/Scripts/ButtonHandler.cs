using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class containing methods for buttons. It is meant to be attatched to button in order to provide it functionality.
/// </summary>
public class ButtonHandler : MonoBehaviour
{
    [SerializeField]
    Animator transition;

    /// <summary>
    /// Method calling an instance of <c>LevelManager</c> to change scene.
    /// </summary>
    /// <param name="sceneIndex">Build index of the scene to be loaded</param>
    public void LoadScene(int sceneIndex)
    {
        LevelManager.instance.LoadScene(sceneIndex, transition);
    }
}
