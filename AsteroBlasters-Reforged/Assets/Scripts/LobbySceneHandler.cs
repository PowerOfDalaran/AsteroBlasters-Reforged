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

    private void Start()
    {
        Lobby currentLobby = LobbyManager.instance.GetLobby();

        lobbyNameText.text = "Lobby name: " + currentLobby.Name;
        lobbyCodeText.text = "Lobby code: " + currentLobby.LobbyCode;
    }
}