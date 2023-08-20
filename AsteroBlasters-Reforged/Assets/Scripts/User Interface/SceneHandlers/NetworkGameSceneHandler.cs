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
    public class NetworkGameSceneHandler : SceneButtonHandler
    {
        // Buttons and other UI elements
        [SerializeField] Button leaveButton;
        [SerializeField] Text timerText;

        // Scoreboard
        [SerializeField] GameObject scoreBoard;
        [SerializeField] GameObject playerScoreUI;
        [SerializeField] GameObject[] positions;

        private GameObject[] playerScoresUI;

        private void Awake()
        {
            leaveButton.onClick.AddListener(() =>
            {
                ChangeButtonsState(false);
                // Logging out of services and leaving scene
                MultiplayerGameManager.instance.RemoveMeServerRpc(MultiplayerGameManager.instance.GetCurrentPlayerData().clientId);
            });
        }

        void Start()
        {
            // Starting the game and adding method to the delegate so, that the scoreboard will update every time network list changes
            MultiplayerGameManager.instance.StartTheGame();
            //MultiplayerGameManager.instance.gameModeManager.OnPlayersGameDataListNetworkListChanged += NetworkGameSceneHandler_OnPlayersKillCountNetworkListChanged;

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
        public void NetworkGameSceneHandler_OnPlayersKillCountNetworkListChanged(object sender, EventArgs e)
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
        /// It clears current player scores and creates new ones
        /// </summary>
        private void UpdateScoreBoard()
        {
            Debug.Log("Updating");
            if (playerScoresUI != null)
            {
                ClearScoreBoard();
            }

            int[] newPlayersOrder = DeathmatchGameManager.instance.OrderThePlayers();
            playerScoresUI = new GameObject[newPlayersOrder.Length];

            for (int i = 0; i < newPlayersOrder.Length; i++)
            {
                playerScoresUI[i] = CreatePlayerScoreUI(i, newPlayersOrder[i]);
            }
        }

        /// <summary>
        /// Method instantiating new <c>playerScoreUI</c> prefab, assigning its position, parent and setting its data.
        /// </summary>
        /// <param name="playerId">Id of player, which data will be displayed</param>
        /// <param name="playerPosition">Id of position, in which prefab will be generated</param>
        /// <returns>Created instance of <c>playerScoreUI</c> prefab</returns>
        private GameObject CreatePlayerScoreUI(int playerId, int playerPosition)
        {
            GameObject newPlayerScoreUI = Instantiate(playerScoreUI);

            newPlayerScoreUI.transform.SetParent(scoreBoard.transform, false);
            newPlayerScoreUI.transform.position = positions[playerPosition].transform.position;

            newPlayerScoreUI.GetComponent<PlayerScore>().SetPlayerData(playerId);
            return newPlayerScoreUI;
        }

        /// <summary>
        /// Method, deleting all current instances of <c>playerScoreUI</c> prefab
        /// </summary>
        private void ClearScoreBoard()
        {
            foreach (GameObject playerScore in playerScoresUI)
            {
                Destroy(playerScore);
            }
        }
    }
}
