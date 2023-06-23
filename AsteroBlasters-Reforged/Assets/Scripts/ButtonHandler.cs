using System;
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

    public void HostGame()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void JoinGame()
    {
        NetworkManager.Singleton.StartClient();
    }
    public void DestroyObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }
    public void SetPlayereRady()
    {
        Debug.Log("player is ready!");
    }
}
