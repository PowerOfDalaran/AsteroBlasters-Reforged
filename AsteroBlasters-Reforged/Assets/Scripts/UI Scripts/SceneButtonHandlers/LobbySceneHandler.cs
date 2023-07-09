using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for communication between UI elements and network scripts
/// </summary>
public class LobbySceneHandler : SceneButtonHandler
{
    // Buttons
    [SerializeField]
    Button startGameButton;
    [SerializeField]
    Button leaveLobbyButton;

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

        // Adding functionality to the buttons
        startGameButton.onClick.AddListener(() =>
        {
            ChangeButtonsState(false);
            LevelManager.instance.NetworkLoadScene("NetworkGameScene");
            LobbyManager.instance.DestroyLobby();
        });

        leaveLobbyButton.onClick.AddListener(() =>
        {
            ChangeButtonsState(false);

            // Logging out of services
            ChangeButtonsState(false);
            MultiplayerGameManager.instance.Leave(MultiplayerGameManager.instance.GetCurrentPlayerData().clientId);


            NetworkManager.Singleton.Shutdown();
            LobbyManager.instance.LeaveLobby();
            AuthenticationService.Instance.SignOut();
            Destroy(NetworkManager.Singleton.gameObject);

            LevelManager.instance.LoadScene("NetworkMenuScene");
        });

        // Setting the start button visibility to true, if current player is a host
        if (NetworkManager.Singleton.IsHost)
        {
            startGameButton.gameObject.SetActive(true);
        }
    }
}
