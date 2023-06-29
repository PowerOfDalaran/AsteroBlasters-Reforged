using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Singleton class responsible for managing the multiplayer game and lobby.
/// </summary>
public class MultiplayerGameManager : NetworkBehaviour
{
    public static MultiplayerGameManager instance;

    [SerializeField]
    List<Color> playerColorList;

    public NetworkList<PlayerData> playerDataNetworkList;
    public event EventHandler OnPlayerDataNetworkListChanged;

    private void Awake()
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

        // Initializating the list of players data and adding an event
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    // METHODS FOR DELEGATES

    /// <summary>
    /// Event, which triggers every time the list of players data is changed
    /// </summary>
    /// <param name="changeEvent"></param>
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Method, which adds new PlayerData to the list and sets its id.
    /// </summary>
    /// <param name="clientId">Id of the player</param>
    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
            colorId = GetFirstUnusedColorId(),
        });
    }

    // STARTING NETWORK MANAGER

    /// <summary>
    /// Method handling creating the game as the host
    /// </summary>
    public void StartAsHost()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.StartHost();
    }

    /// <summary>
    /// Method handling joining the game as client.
    /// </summary>
    public void StartAsClient()
    {
        NetworkManager.StartClient();
    }

    /// <summary>
    /// Method checking if player with given index is connected.
    /// </summary>
    /// <param name="playerIndex">Player index</param>
    /// <returns></returns>
    public bool IsPlayerIndexConnected(int playerIndex) 
    { 
        return playerIndex < playerDataNetworkList.Count;
    }

    // PLAYER DATA, INDEX ETC.

    /// <summary>
    /// Method returning the index of the player in <c>playerDataNetworkList</c> array
    /// </summary>
    /// <param name="clientId">Id of the player, whose index you want to access</param>
    /// <returns>Index of player with given id</returns>
    public int GetPlayerIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Method returning the <c>PlayerData</c> object of player with given id
    /// </summary>
    /// <param name="clientId">Id of the player, whose data you want to get</param>
    /// <returns><c>PlayerData</c> object of player with given id</returns>
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    /// <summary>
    /// Method returning the <c>PlayerData</c> object of local player
    /// </summary>
    /// <returns><c>PlayerData</c> object of the local player</returns>
    public PlayerData GetCurrentPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    /// <summary>
    /// Method returning the <c>PlayerData</c> object of player with given index
    /// </summary>
    /// <param name="playerIndex">Index of player, whose data you want to access</param>
    /// <returns>Data of player with given id</returns>
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }


    // PLAYERS COLORS

    /// <summary>
    /// Method returning the color with given index from <c>playerColorList</c>
    /// </summary>
    /// <param name="colorId">Id of color you want to access</param>
    /// <returns>Color with given id</returns>
    public Color GetPlayerColor(int colorId)
    {
        return playerColorList[colorId];
    }

    /// <summary>
    /// Method iterating on every player in the lobby and checking if their color is equal to given one
    /// </summary>
    /// <param name="colorId">Id of color, which availability you want to check</param>
    /// <returns>True if color is available, false if its not</returns>
    private bool IsColorAvailable(int colorId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.colorId == colorId)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Method iterating on list of colors and checking on every one if its available
    /// </summary>
    /// <returns>Id of first available color, -1 if every color is occupied</returns>
    int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Method calling the <c>ChangePlayerColorServerRpc</c> to change the color of player
    /// </summary>
    /// <param name="colorId">Color, which you want to apply to the player</param>
    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    /// <summary>
    /// Server Rpc method, which basing on <c>ServerRpcParams</c> defines, which players called the method, checks if given color is available and changes his colorId in <c>playerDataNetworkList</c>
    /// </summary>
    /// <param name="colorId">Id of color we want to change to</param>
    /// <param name="serverRpcParams">Default parameters of Server Rpc, which allows to decide which player want to change their colors</param>
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            return;
        }

        int playerDataIndex = GetPlayerIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.colorId = colorId;
        playerDataNetworkList[playerDataIndex] = playerData;
    }
}
