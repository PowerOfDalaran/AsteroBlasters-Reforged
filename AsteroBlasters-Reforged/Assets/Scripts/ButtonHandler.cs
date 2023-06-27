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
    InputField playerNameInputField;
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
    /// <summary>
    /// Method calling the <c>LobbyManager</c> class to create new lobby with given parameters.
    /// </summary>
    public void CreateLobby()
    {
       LobbyManager.instance.CreateLobby(lobbyNameInputField.text, (int) maxPlayersSlider.value);
    }

    /// <summary>
    /// Method calling the <c>LobbyManager</c> class to join lobby with given code.
    /// </summary>
    public void JoinLobby()
    {
        LobbyManager.instance.JoinLobbyByCode(lobbyCodeInputField.text);
    }

    /// <summary>
    /// Method calling the <c>LobbyManager</c> to set the name of current player.
    /// </summary>
    public void SelectPlayerName()
    {
        LobbyManager.instance.playerName = playerNameInputField.text;
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
