using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using DataStructure;
using SceneManagment;
using NetworkFunctionality;
using PlayerFunctionality;
using System.Collections;

namespace GameManager
{
    /// <summary>
    /// Singleton class managing the game mechanics in deathmatch game mode (does not move between scenes)
    /// </summary>
    public class DeathmatchGameManager : NetworkBehaviour
    {
        public static DeathmatchGameManager instance;

        [SerializeField] GameObject matchDataPrefab;

        public NetworkList<PlayerGameData> playersGameDataList;
        public NetworkVariable<float> timeLeft;

        public event EventHandler OnPlayersGameDataListNetworkListChanged;

        private float timeLimit;
        private bool gameActive = true;

        #region Build-in methods
        private void Awake()
        {
            // Assigning this instance to variable, creating network variables and adding the methodS to the events
            instance = this;

            playersGameDataList = new NetworkList<PlayerGameData>();
            timeLeft = new NetworkVariable<float>();

            playersGameDataList.OnListChanged += PlayersGameDataList_OnListChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;
        }

        private void Start()
        {
            if (IsHost)
            {
                // Adding the methods to the events
                NetworkPlayerController.onPlayerDeath += UpdateStats;

                // Setting up the playersKillCount (Network List) and (Network Variable) timeLeft
                foreach (PlayerNetworkData playerNetworkData in MultiplayerGameManager.instance.playerDataNetworkList)
                {
                    playersGameDataList.Add(new PlayerGameData
                    {
                        playerId = playerNetworkData.clientId,
                        killCount = 0,
                        deathCount = 0
                    });
                }

                timeLeft.Value = 45f;
            }

            timeLimit = timeLeft.Value;
        }

        private void FixedUpdate()
        {
            // Updating timer
            if (IsHost && gameActive)
            {
                timeLeft.Value -= Time.deltaTime;

                if (timeLeft.Value <= 0)
                {
                    EndGameClientRpc(UtilitiesToolbox.ListToArray(UtilitiesToolbox.NetworkListPGDToListPGD(playersGameDataList)));
                }
            }
        }
        #endregion

        #region KillCount List
        /// <summary>
        /// Method, which acts as an connection between an <c>OnPlayersKillCountNetworkListChanged</c> event and other methods.
        /// </summary>
        /// <param name="changeEvent"></param>
        private void PlayersGameDataList_OnListChanged(NetworkListEvent<PlayerGameData> changeEvent)
        {
            OnPlayersGameDataListNetworkListChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method, which activated if client is disconnected from the host, deleting the leaving player from the network list
        /// </summary>
        /// <param name="clientId">Id of client, who disconnected</param>
        public void NetworkManager_OnClientDisconnectedCallback(ulong clientId)
        {
            Debug.Log("aktywowano");
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log("jest host");
                foreach (PlayerGameData playerGameData in playersGameDataList)
                {
                    if (playerGameData.playerId == clientId)
                    {
                        playersGameDataList.Remove(playerGameData);
                    }
                }
            }
        }

        /// <summary>
        /// Method adding one kill to the killing player, one death to killed player, and then checking if game should be ended.
        /// </summary>
        /// <param name="killedPlayerIndex">Index of player, who was killed</param>
        /// <param name="killingPlayerIndex">Index of player, who killed</param>
        public void UpdateStats(int killedPlayerIndex, int killingPlayerIndex)
        {
            PlayerGameData killingPlayerGameData = GetPlayerGameDataFromIndex(killingPlayerIndex);
            killingPlayerGameData.killCount += 1;
            playersGameDataList[killingPlayerIndex] = killingPlayerGameData;

            PlayerGameData killedPlayerGameData = GetPlayerGameDataFromIndex(killedPlayerIndex);
            killedPlayerGameData.deathCount += 1;
            playersGameDataList[killedPlayerIndex] = killedPlayerGameData;

            if (playersGameDataList[killingPlayerIndex].killCount == 3 && gameActive)
            {
                EndGameClientRpc(UtilitiesToolbox.ListToArray(UtilitiesToolbox.NetworkListPGDToListPGD(playersGameDataList)));
            }
        }
        #endregion

        #region Get Player Data
        /// <summary>
        /// Method creating simple List of players kills, with indexes being index of players in <c>playersGameDataList</c>
        /// </summary>
        /// <returns>Int type List, with players kills</returns>
        public List<int> GetPlayersKillCountList()
        {
            List<int> resultArray = new List<int>();

            foreach (PlayerGameData playerGameData in playersGameDataList)
            {
                resultArray.Add(playerGameData.killCount);
            }

            return resultArray;
        }

        /// <summary>
        /// Method creating simple List of players kills, with indexes being index of players in given <c>PlayerGameData</c> type Array.
        /// This method exists to handle the situation, in which client needs to work with List passed down by host.
        /// </summary>
        /// <param name="playerGameDatas"></param>
        /// <returns></returns>
        public List<int> GetPlayersKillCountList(PlayerGameData[] playerGameDatas)
        {
            List<int> resultArray = new List<int>();

            foreach (PlayerGameData playerGameData in playerGameDatas)
            {
                resultArray.Add(playerGameData.killCount);
            }

            return resultArray;
        }

        /// <summary>
        /// Method returning the <c>PlayerGameData</c> object corresponding to player with given index
        /// </summary>
        /// <param name="playerIndex">Index of player, which data we want to get</param>
        /// <returns>Data of player with given index</returns>
        public PlayerGameData GetPlayerGameDataFromIndex(int playerIndex) 
        {
            return playersGameDataList[playerIndex];
        }
        public int GetPlayerIndex(ulong playerId)
        {
            for (int i = 0; i < playersGameDataList.Count; i++)
            {
                if (playersGameDataList[i].playerId == playerId)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Method returning the <c>PlayerGameData</c> object, corresponding to player with given id
        /// </summary>
        /// <param name="playerId">Id of player, which data we want to get</param>
        /// <returns>Data of player with given id</returns>
        public PlayerGameData GetPlayerGameDataFromId(ulong playerId)
        {
            for (int i = 0; i < playersGameDataList.Count; i++)
            {
                if (playersGameDataList[i].playerId == playerId)
                {
                    return playersGameDataList[i];
                }
            }

            return default;
        }
        #endregion

        /// <summary>
        /// Method ordering the players by their score.
        /// </summary>
        /// <returns>The array, in which every index corresponds to player index and value is equal to their correct order.</returns>
        public int[] OrderThePlayers()
        {
            List<int> newList = GetPlayersKillCountList();
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
        /// Method ordering the players by their score. This overload however, instead of using this client <c>playersGameDataList</c>, uses passed down array of <c>playerGameData</c> objects.
        /// </summary>
        /// <param name="gameResult">Array of <c>playerGameData</c> objects. The name origins from the fact that this overloaded method is being used only when the game is ending.</param>
        /// <returns>The array, in which every index corresponds to player index and value is equal to their correct order.</returns>
        public int[] OrderThePlayers(PlayerGameData[] gameResult)
        {
            List<int> playersKillCountList = GetPlayersKillCountList(gameResult);
            int[] orderedPlayers = new int[playersKillCountList.Count];

            for (int i = 0; i < playersKillCountList.Count; i++)
            {
                int highestValue = playersKillCountList.Max();
                int highestValueIndex = playersKillCountList.IndexOf(highestValue);

                orderedPlayers[highestValueIndex] = i;
                playersKillCountList[highestValueIndex] = -1;
            }

            return orderedPlayers;
        }

        /// <summary>
        /// Client Rpc method, which activated on every client executes endgame mechanics.
        /// </summary>
        /// <param name="gameResult">The array of <c>PlayerGameData</c> objects, passed by host to every client</param>
        [ClientRpc]
        void EndGameClientRpc(PlayerGameData[] gameResult)
        {
            // Turning gameActive bools off
            gameActive = false;
            MultiplayerGameManager.instance.gameActive = false;

            // Creating the player data array and getting the proper order of players
            object[][] playerDataArray = new object[MultiplayerGameManager.instance.playerDataNetworkList.Count][];
            int[] playersRanking = OrderThePlayers(gameResult);

            // For each player in the game, creating new entry in the data object and assinging values to it
            for (int i = 0; i < MultiplayerGameManager.instance.playerDataNetworkList.Count; i++)
            {
                PlayerNetworkData playerData = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(i);

                object[] playerSubArray = new object[3];
                playerSubArray[0] = playerData.playerName.ToString();
                playerSubArray[1] = playersRanking[i];
                playerSubArray[2] = UtilitiesToolbox.GetStringFromColor(MultiplayerGameManager.instance.GetPlayerColor(playerData.colorId));

                playerDataArray[i] = playerSubArray;
            }

            // Turning off and destroying game objects responsible for network connections etc.
            UtilitiesToolbox.DeleteNetworkConnections(true, true, true, true);

            // Creating data object, assiging values to it and loading new scene.
            GameObject newMatchData = Instantiate(matchDataPrefab);
            newMatchData.GetComponent<MatchData>().SetData(playerDataArray, timeLimit.ToString());
            LevelManager.instance.LoadScene("MatchResultScene");
        }
    }
}