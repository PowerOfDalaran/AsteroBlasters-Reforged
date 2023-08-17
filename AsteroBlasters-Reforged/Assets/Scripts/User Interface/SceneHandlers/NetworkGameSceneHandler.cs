using System;
using UnityEngine;
using UnityEngine.UI;
using NetworkFunctionality;
using GameManager;
namespace UserInterface
{
    /// <summary>
    /// Class responsible for managing the UI in <c>NetworkGame</c> scene.
    /// </summary>
    public class NetworkGameSceneHandler : MonoBehaviour
    {
        [SerializeField]
        Text timerText;

        [SerializeField]
        GameObject scoreBoard;
        [SerializeField]
        GameObject playerScoreUI;
        [SerializeField]
        GameObject[] positions;

        void Start()
        {
            MultiplayerGameManager.instance.StartTheGame();
            DeathmatchGameManager.instance.OnPlayersGameDataListNetworkListChanged += NetworkGameSceneHandler_OnPlayersKillCountNetworkListChanged;

            UpdateScoreBoard();
        }

        private void Update()
        {
            UpdateTimer();
        }

        /// <summary>
        /// Method created as intersection between the <c>OnPlayersKillCountNetworkListChanged</c> delegate and this class method that are supposed to run with this event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NetworkGameSceneHandler_OnPlayersKillCountNetworkListChanged(object sender, System.EventArgs e)
        {
            UpdateScoreBoard();
        }

        /// <summary>
        /// Method getting the <c>timeLeft</c> value, transforms it to proper strings and sets the timer's text to formed string
        /// </summary>
        private void UpdateTimer()
        {
            TimeSpan timerTimeSpan = TimeSpan.FromSeconds(DeathmatchGameManager.instance.timeLeft.Value);
            timerText.text = UtilitiesToolbox.GetTimeAsString(timerTimeSpan);
        }

        /// <summary>
        /// Method being called everytime the playersKillCount list changes.
        /// It runs trough the order list and calls <c>CreatePlayerScoreUI</c> with proper position and id.
        /// </summary>
        private void UpdateScoreBoard()
        {
            int[] newPlayersOrder = DeathmatchGameManager.instance.OrderThePlayers();

            for (int i = 0; i < newPlayersOrder.Length; i++)
            {
                CreatePlayerScoreUI(i, newPlayersOrder[i]);
            }
        }

        /// <summary>
        /// Method instantiating new <c>playerScoreUI</c> prefab, assigning its position, parent and setting its data.
        /// </summary>
        /// <param name="playerId">Id of player, which data will be displayed</param>
        /// <param name="playerPosition">Id of position, in which prefab will be generated</param>
        private void CreatePlayerScoreUI(int playerId, int playerPosition)
        {
            GameObject newPlayerScoreUI = Instantiate(playerScoreUI);

            newPlayerScoreUI.transform.SetParent(scoreBoard.transform, false);
            newPlayerScoreUI.transform.position = positions[playerPosition].transform.position;

            newPlayerScoreUI.GetComponent<PlayerScore>().SetPlayerData(playerId);
        }
    }
}
