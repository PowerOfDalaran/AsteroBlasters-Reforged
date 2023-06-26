using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

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

        await UnityServices.InitializeAsync();


        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void FixedUpdate()
    {
        LobbyHeartbeat();
        if (hostedLobby != null)
        {
            Debug.Log(hostedLobby.Players.Count);
        }
    }

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
