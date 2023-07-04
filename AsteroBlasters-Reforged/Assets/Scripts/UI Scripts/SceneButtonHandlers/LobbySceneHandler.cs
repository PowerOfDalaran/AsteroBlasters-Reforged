using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for communication between UI elements and network scripts
/// </summary>
public class LobbySceneHandler : MonoBehaviour
{
    // Buttons
    [SerializeField]
    Button[] buttons;

    [SerializeField]
    Button startGameButton;

    // Other UI elements
    [SerializeField]
    Text lobbyNameText;
    [SerializeField]
    Text lobbyCodeText;
    [SerializeField]
    GameObject LoadingScreen;

    private void Awake()
    {
        // Setting the lobby UI to proper values
        Lobby currentLobby = LobbyManager.instance.GetLobby();

        lobbyNameText.text = "Lobby name: " + currentLobby.Name;
        lobbyCodeText.text = "Lobby code: " + currentLobby.LobbyCode;

        startGameButton.onClick.AddListener(() =>
        {
            TurnOffButtons();
            Animator transition = LoadingScreen.GetComponent<Animator>();
            LevelManager.instance.NetworkLoadScene("NetworkGameScene", transition);
        });

        // Setting the start button visibility to true, if current player is a host
        if (NetworkManager.Singleton.IsHost)
        {
            startGameButton.gameObject.SetActive(true);
        }
    }

    private void TurnOffButtons()
    {
        foreach (Button button in buttons)
        {
            button.enabled = false;
        }
    }
}
