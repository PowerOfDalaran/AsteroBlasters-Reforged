using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Singleton class responsible for managing the multiplayer game and lobby.
/// </summary>
public class MultiplayerGameManager : NetworkBehaviour
{
    public static MultiplayerGameManager instance;

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

    /// <summary>
    /// Event, which triggers every time the list of players data is changed
    /// </summary>
    /// <param name="changeEvent"></param>
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

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
    /// Method, which adds new PlayerData to the list and sets its id.
    /// </summary>
    /// <param name="clientId">Id of the player</param>
    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
        });
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
}
