using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for communication between UI elements and network scripts
/// </summary>
public class LobbySceneHandler : MonoBehaviour
{
    [SerializeField]
    Text lobbyNameText;
    [SerializeField]
    Text lobbyCodeText;
    [SerializeField]
    Button startGameButton;

    private void Start()
    {
        // Setting the lobby UI to proper values
        Lobby currentLobby = LobbyManager.instance.GetLobby();

        lobbyNameText.text = "Lobby name: " + currentLobby.Name;
        lobbyCodeText.text = "Lobby code: " + currentLobby.LobbyCode;

        // Setting the start button visibility to true, if current player is a host
        if (NetworkManager.Singleton.IsHost)
        {
            startGameButton.gameObject.SetActive(true);
        }
    }
}
