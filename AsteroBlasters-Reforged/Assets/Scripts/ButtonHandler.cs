using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Class containing methods for buttons. It is meant to be attatched to button in order to provide it functionality.
/// </summary>
public class ButtonHandler : MonoBehaviour
{
    /// <summary>
    /// Method calling an instance of <c>LevelManager</c> to change scene.
    /// </summary>
    /// <param name="sceneIndex">Build index of the scene to be loaded</param>
    public void LoadScene(int sceneIndex)
    {
        LevelManager.instance.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Method calling the NetworkManager to create new session as host
    /// </summary>
    public void HostGame()
    {
        NetworkManager.Singleton.StartHost();
    }

    /// <summary>
    /// Method calling the NetworkManager to create new session as client
    /// </summary>
    public void JoinGame()
    {
        NetworkManager.Singleton.StartClient();
    }

    /// <summary>
    /// Method destroying chosen game object
    /// </summary>
    /// <param name="gameObject">Gameobject that you want to destroy</param>
    public void DestroyObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}
