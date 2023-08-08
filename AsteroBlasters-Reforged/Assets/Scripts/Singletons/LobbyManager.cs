using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

/// <summary>
/// Class responsible for managing connection with unity lobby service.
/// </summary>
public class LobbyManager : MonoBehaviour
{
    const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    public static LobbyManager instance;
    Lobby hostedLobby;
    Lobby joinedLobby;
    float heartbeatTimer;
    float maxHeartbeatTimer = 15f;
    public string playerName;

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
    /// Method calling the Unity Relay Service to create new allocation
    /// </summary>
    /// <param name="maxPlayers">Maximum number of players (with host)</param>
    /// <returns>Created allocation</returns>
    async Task<Allocation> AllocateRelay(int maxPlayers)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            return allocation;
        }
        catch (RelayServiceException exception)
        {
            Debug.Log(exception);
            return default;
        }
    }

    /// <summary>
    /// Method calling the Unity Relay Service to get join code of given allocation
    /// </summary>
    /// <param name="allocation">Allocation you want to get code of</param>
    /// <returns>Joining code of given allocation</returns>
    async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException exception)
        {
            Debug.Log(exception);
            return default;
        }

    }

    /// <summary>
    /// Method accessing the allocation needed to join the host, by calling Unity Relay Service and giving it joining code
    /// </summary>
    /// <param name="relayJoinCode">Code generated by Unity Relay Service</param>
    /// <returns>Allocation needed to join the host</returns>
    async Task<JoinAllocation> JoinRelay(string relayJoinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            return joinAllocation;
        }
        catch (RelayServiceException exception)
        {
            Debug.Log(exception);
            return default;
        }
    }

    /// <summary>
    /// Method creating lobby with given name and maximum number of players.
    /// </summary>
    /// <param name="lobbyName">Name of new lobby</param>
    /// <param name="maxPlayers">Maximum number of players, which can join the lobby</param>
    public async Task<bool> CreateLobby(string lobbyName, int maxPlayers)
    {
        try
        {
            // Creating "options" for lobby, which carry extra data and creating new lobby
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                    }
                }
            };

            hostedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);
            joinedLobby = hostedLobby;

            // Creating new allocation, accessing its relay joining code and adding it to lobby data
            Allocation allocation = await AllocateRelay(maxPlayers);
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(hostedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            // Setting the NetworkManager to use the created allocation and starting it (as host)
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            MultiplayerGameManager.instance.StartAsHost();
            return true;
        }
        catch (LobbyServiceException exception)
        {
            Debug.LogException(exception);
            return false;
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
    public async Task<bool> JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            // Creating "options" for lobby, which carry extra data and creating new lobby
            JoinLobbyByCodeOptions lobbyOptions = new JoinLobbyByCodeOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                    }
                }
            };

            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, lobbyOptions);

            // Accessing the relay code from lobby data, creating new allocation and connecting to host with it
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            // Starting the NetworkManager (as client)
            MultiplayerGameManager.instance.StartAsClient();
            return true;
        }
        catch (LobbyServiceException exception)
        {
            Debug.Log(exception);
            return false;
        }
    }

    /// <summary>
    /// Method returning the lobby, in which player currently is
    /// </summary>
    /// <returns>The lobby object</returns>
    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    /// <summary>
    /// Method removing current player from the lobby
    /// </summary>
    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
            hostedLobby = null;
        }
        catch (LobbyServiceException exception)
        {
            Debug.Log(exception);
        }
    }

    /// <summary>
    /// Method deleting the lobby from Unity Services and removing its references.
    /// FOR HOST ONLY!
    /// </summary>
    public async void DestroyLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(hostedLobby.Id);
            joinedLobby = null;
            hostedLobby = null;
        }
        catch (LobbyServiceException exception)
        {
            Debug.Log(exception);
        }
    }
}