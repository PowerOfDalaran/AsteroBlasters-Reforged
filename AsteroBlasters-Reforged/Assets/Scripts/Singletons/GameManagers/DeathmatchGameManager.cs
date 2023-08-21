using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using DataStructure;
using SceneManagment;
using NetworkFunctionality;

namespace GameManager
{
    /// <summary>
    /// Singleton class managing the game mechanics in deathmatch game mode (does not move between scenes)
    /// </summary>
    public class DeathmatchGameManager : GameModeManager
    {
        public static DeathmatchGameManager instance;

        public NetworkVariable<float> timeLeft;

        #region Build-in methods
        protected override void Awake()
        {
            base.Awake();

            instance = this;
            timeLeft = new NetworkVariable<float>();
        }

        protected override void Start()
        {
            base.Start();

            // Setting up the timer
            if (NetworkManager.IsHost)
            {
                timeLeft.Value = MultiplayerGameManager.instance.timeLimit.Value;
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
                    EndGameClientRpc(UtilitiesToolbox.ListToArray(UtilitiesToolbox.NetworkListPGDToListPGD(playersGameDataList)));
                }
            }
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
        protected override void EndGameClientRpc(PlayerGameData[] gameResult)
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
            newMatchData.GetComponent<MatchData>().SetData(playerDataArray, MultiplayerGameManager.instance.timeLimit.Value.ToString());
            LevelManager.instance.LoadScene("MatchResultScene");
        }
    }
}