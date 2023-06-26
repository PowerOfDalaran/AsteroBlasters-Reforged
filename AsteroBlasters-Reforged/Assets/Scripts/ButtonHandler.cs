using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class containing methods for buttons. It is meant to be attatched to button in order to provide it functionality.
/// </summary>
public class ButtonHandler : MonoBehaviour
{
    GameObject LoadingScreen;

    [SerializeField]
    Slider maxPlayersSlider;
    [SerializeField]
    InputField lobbyNameInputField;
    [SerializeField]
    InputField lobbyCodeInputField;

    public void Awake()
    {
        LoadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen");
    }

    /// <summary>
    /// Method calling an instance of <c>LevelManager</c> to change scene.
    /// </summary>
    /// <param name="sceneIndex">Build index of the scene to be loaded</param>
    public void LoadScene(int sceneIndex)
    {
        Animator transition = LoadingScreen.GetComponent<Animator>();
        LevelManager.instance.LoadScene(sceneIndex, transition);
    }

    public void CreateLobby()
    {
       LobbyManager.instance.CreateLobby(lobbyNameInputField.text, (int) maxPlayersSlider.value);
    }

    public void JoinLobby()
    {
        LobbyManager.instance.JoinLobbyByCode(lobbyCodeInputField.text);
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

    public void SetPlayereRady()
    {
        Debug.Log("player is ready!");
    }
}
