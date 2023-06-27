using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelect : NetworkBehaviour
{
    public static CharacterSelect instance;
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

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this,EventArgs.Empty);
    }

    public bool IsPlayerIndexConnected(int playerIndex) 
    { 
        return playerIndex < playerDataNetworkList.Count;
    }
}
