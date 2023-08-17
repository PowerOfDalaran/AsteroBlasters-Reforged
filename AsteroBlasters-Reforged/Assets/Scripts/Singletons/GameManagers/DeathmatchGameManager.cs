using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using DataStructure;
using SceneManagment;
using NetworkFunctionality;
using PlayerFunctionality;

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
        private float timeLimit;
        public NetworkVariable<float> timeLeft;

        public event EventHandler OnPlayersGameDataListNetworkListChanged;

        private bool gameActive = true;

        #region Build-in methods
        private void Awake()
        {
            instance = this;

            playersGameDataList = new NetworkList<PlayerGameData>();
            playersGameDataList.OnListChanged += PlayersGameDataList_OnListChanged;

            timeLeft = new NetworkVariable<float>();
        }

        private void Start()
        {
            // Setting up the playersKillCount (Network List) and (Network Variable) timeLeft
            if (IsHost)
            {
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

        private void OnEnable()
        {
            if (IsHost)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;
                NetworkPlayerController.onPlayerDeath += UpdateStats;
                //foreach(GameObject playerGameObject in MultiplayerGameManager.instance.playerCharacters)
                //{
                //    playerGameObject.GetComponent<NetworkPlayerController>().onPlayerDeath += UpdateStats;
                //}
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
        /// Method returning <c>playerKillCount</c> Network List object in form of standard List.
        /// </summary>
        /// <returns><c>playerKillCount</c> as List object</returns>
        public List<int> GetPlayersKillCountList()
        {
            List<int> resultArray = new List<int>();

            foreach (PlayerGameData playerGameData in playersGameDataList)
            {
                resultArray.Add(playerGameData.killCount);
            }

            return resultArray;
        }

        private void NetworkManager_OnClientDisconnectedCallback(ulong clientId)
        {
            if (NetworkManager.Singleton.IsHost)
            {
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
        /// Server Rpc method adding one point to the player with given id.
        /// </summary>
        /// <param name="playerIndex">Id of player, which scored the point</param>
        public void UpdateStats(int killedPlayerIndex, int killingPlayerIndex)
        {
            //PlayerNetworkData killingPlayerNetworkData = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(killingPlayerIndex);
            //PlayerNetworkData killedPlayerNetworkData = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(killedPlayerIndex);

            PlayerGameData killingPlayerGameData = GetPlayerGameDataFromIndex(killingPlayerIndex);
            killingPlayerGameData.killCount += 1;
            playersGameDataList[killingPlayerIndex] = killingPlayerGameData;

            PlayerGameData killedPlayerGameData = GetPlayerGameDataFromIndex(killedPlayerIndex);
            killedPlayerGameData.killCount += 1;
            playersGameDataList[killedPlayerIndex] = killedPlayerGameData;

            if (playersGameDataList[killingPlayerIndex].killCount == 2 && gameActive)
            {
                EndGameClientRpc();
            }
        }
        #endregion

        #region Get Player Data
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

        #endregion
        /// <summary>
        /// Client Rpc method, which activated on every client executes endgame mechanics
        /// </summary>
        [ClientRpc]
        void EndGameClientRpc()
        {
            gameActive = false;
            MultiplayerGameManager.instance.gameActive = false;

            // Creating the player data array and getting the proper order of players
            object[][] playerDataArray = new object[MultiplayerGameManager.instance.playerDataNetworkList.Count][];
            int[] playersRanking = OrderThePlayers();

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