using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

/// <summary>
/// Class responsible for managing connection with unity lobby service.
/// </summary>
public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;
    Lobby hostedLobby;
    Lobby joinedLobby;
    float heartbeatTimer;
    float maxHeartbeatTimer = 15f;

    private async void Awake()
    {
        // Checking if another instance of this class don't exist yet and deleting itself if that is the case
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Starting and connecting to lobby services
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void FixedUpdate()
    {
        LobbyHeartbeat();
    }

    /// <summary>
    /// Method creating lobby with given name and maximum number of players.
    /// </summary>
    /// <param name="lobbyName">Name of new lobby</param>
    /// <param name="maxPlayers">Maximum number of players, which can join the lobby</param>
    public async void CreateLobby(string lobbyName, int maxPlayers)
    {
        try
        {
            hostedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
            joinedLobby = hostedLobby;
            Debug.Log(hostedLobby.Name + "; " + hostedLobby.Players.Count + "/" + hostedLobby.MaxPlayers + "; " + hostedLobby.LobbyCode);
        }
        catch (LobbyServiceException exception)
        {
            Debug.LogException(exception);
        }
    }

    /// <summary>
    /// Method pinging the hosted lobby, in order to not let it close itself.
    /// </summary>
    async void LobbyHeartbeat()
    {
        if (hostedLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                heartbeatTimer = maxHeartbeatTimer;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostedLobby.Id);
            }

        }
    }

    /// <summary>
    /// Method joining the lobby with certain code.
    /// </summary>
    /// <param name="lobbyCode">Code of lobby you want to join</param>
    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode);
            Debug.Log("Lobby joined!\r\n" + joinedLobby.Name);
        }
        catch (LobbyServiceException exception)
        {
            Debug.Log(exception);
        }
    }
}
