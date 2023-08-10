using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Singleton class managing the game mechanics in deathmatch game mode (does not move between scenes)
/// </summary>
public class DeathmatchGameManager : NetworkBehaviour
{
    public static DeathmatchGameManager instance;

    [SerializeField]
    GameObject matchDataPrefab;

    private NetworkList<int> playersKillCount = new NetworkList<int>();
    private float timeLimit;
    public NetworkVariable<float> timeLeft = new NetworkVariable<float>();

    public event EventHandler OnPlayersKillCountNetworkListChanged;

    private bool gameActive = true;

    private void Start()
    {
        instance = this;

        // Setting up the playersKillCount (Network List) and (Network Variable) timeLeft
        if (IsHost)
        {
            foreach (PlayerData playerData in MultiplayerGameManager.instance.playerDataNetworkList) 
            {
                playersKillCount.Add(0);
            }

            timeLeft.Value = 3f;
        }

        playersKillCount.OnListChanged += PlayersKillCount_OnListChanged;
        timeLimit = timeLeft.Value;
    }

    /// <summary>
    /// Method returning <c>playerKillCount</c> Network List object in form of standard List.
    /// </summary>
    /// <returns><c>playerKillCount</c> as List object</returns>
    public List<int> GetPlayersKillCount()
    {
        List<int> resultArray = new List<int>();

        foreach (int i in playersKillCount)
        {
            resultArray.Add(i);
        }

        return resultArray;
    }

    /// <summary>
    /// Method ordering the players by their score.
    /// </summary>
    /// <returns>The array, in which every index corresponds to player index and value is equal to their correct order.</returns>
    public int[] OrderThePlayers()
    {
        List<int> newList = GetPlayersKillCount();
        int[] orderedPlayers = new int[newList.Count];

        for (int i = 0; i < newList.Count; i++)
        {
            int highestValue = newList.Max();
            int highestValueIndex = newList.IndexOf(highestValue);

            orderedPlayers[highestValueIndex] = i;
            newList[highestValueIndex] = -1;
        }

        return orderedPlayers;
    }

    /// <summary>
    /// Method, which acts as an connection between an <c>OnPlayersKillCountNetworkListChanged</c> event and other methods.
    /// </summary>
    /// <param name="changeEvent"></param>
    private void PlayersKillCount_OnListChanged(NetworkListEvent<int> changeEvent)
    {
        OnPlayersKillCountNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Server Rpc method adding one point to the player with given id.
    /// </summary>
    /// <param name="playerIndex">Id of player, which scored the point</param>
    [ServerRpc]
    public void AddKillCountServerRpc(int playerIndex)
    {
        playersKillCount[playerIndex] += 1;

        if (playersKillCount[playerIndex] == 1 && gameActive)
        {
            EndGameClientRpc();
        }
    }

    private void FixedUpdate()
    {
        // Updating timer
        if (IsHost && gameActive)
        {
            timeLeft.Value -= Time.deltaTime;

            if (timeLeft.Value <= 0)
            {
                EndGameClientRpc();
                gameActive = false;
            }
        }
    }

    [ClientRpc]
    void EndGameClientRpc()
    {
        NetworkManager.Singleton.Shutdown();

        Destroy(MultiplayerGameManager.instance.gameObject);
        Destroy(LobbyManager.instance.gameObject);
        Destroy(NetworkManager.Singleton.gameObject);

        object[][] playerDataArray = new object[MultiplayerGameManager.instance.playerDataNetworkList.Count][];
        int[] playersRanking = OrderThePlayers();

        for (int i = 0; i < MultiplayerGameManager.instance.playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(i);

            object[] playerSubArray = new object[3];
            playerSubArray[0] = playerData.playerName.ToString();
            playerSubArray[1] = playersRanking[i];
            playerSubArray[2] = UtilitiesToolbox.GetStringFromColor(MultiplayerGameManager.instance.GetPlayerColor(playerData.colorId));

            playerDataArray[i] = playerSubArray;
        }

        GameObject newMatchData = Instantiate(matchDataPrefab);
        newMatchData.GetComponent<MatchData>().SetData(playerDataArray, timeLimit.ToString());
        LevelManager.instance.LoadScene("MatchResultScene");
    }
}
