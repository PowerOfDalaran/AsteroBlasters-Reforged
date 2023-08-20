using DataStructure;
using NetworkFunctionality;
using PlayerFunctionality;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GameManager
{
    /// <summary>
    /// Parent class for all Game Mode Managers, takes care of universal functionalities like keeping the player game data list etc.
    /// </summary>
    public class GameModeManager : NetworkBehaviour
    {
        [SerializeField] protected GameObject matchDataPrefab;

        public NetworkList<PlayerGameData> playersGameDataList;

        public event EventHandler OnPlayersGameDataListNetworkListChanged;

        protected bool gameActive = true;

        protected virtual void Awake()
        {
            // Creating new Network List and adding methods to the delegate
            playersGameDataList = new NetworkList<PlayerGameData>();

            playersGameDataList.OnListChanged += PlayersGameDataList_OnListChanged;
            MultiplayerGameManager.instance.gameModeManager = this;
        }

        protected virtual void Start()
        {

            if (NetworkManager.IsHost)
            {
                // Adding the method to the events
                NetworkPlayerController.onPlayerDeath += UpdateStats;

                CreateNetworkList(MultiplayerGameManager.instance.playerDataNetworkList);

                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;            }
        }

        /// <summary>
        /// Method, which acts as an connection between an <c>OnPlayersKillCountNetworkListChanged</c> event and other methods.
        /// </summary>
        /// <param name="changeEvent"></param>
        private void PlayersGameDataList_OnListChanged(NetworkListEvent<PlayerGameData> changeEvent)
        {
            OnPlayersGameDataListNetworkListChanged?.Invoke(this, EventArgs.Empty);
        }

        public void CreateNetworkList(NetworkList<PlayerNetworkData> playerNetworkDataList)
        {
            foreach (PlayerNetworkData playerNetworkData in playerNetworkDataList)
            {
                playersGameDataList.Add(new PlayerGameData
                {
                    playerId = playerNetworkData.clientId,
                    killCount = 0,
                    deathCount = 0,
                });
            }
        }

        /// <summary>
        /// Method, which activated if client is disconnected from the host, deleting the leaving player from the network list
        /// </summary>
        /// <param name="clientId">Id of client, who disconnected</param>
        public void NetworkManager_OnClientDisconnectedCallback(ulong clientId)
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

        #region Get Player Data
        /// <summary>
        /// Method creating simple List of players kills, with indexes are index of players in <c>playersGameDataList</c>
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
        /// Client Rpc method, which activated on every client executes endgame mechanics.
        /// Must be overrided by child scripts.
        /// </summary>
        /// <param name="matchResult">The array of <c>PlayerGameData</c> objects, passed by host to every client</param>
        [ClientRpc]
        protected virtual void EndGameClientRpc(PlayerGameData[] matchResult) { }
    }
}
