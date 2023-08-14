using UnityEngine;
using UnityEngine.UI;
using DataStructure;
using NetworkFunctionality;
using GameManager;

namespace UserInterface
{
    /// <summary>
    /// Class attatched to PlayerScoreUI prefab, responsible for properly assign its data in UI
    /// </summary>
    public class PlayerScore : MonoBehaviour
    {
        [SerializeField]
        Text scoreText;
        [SerializeField]
        Text playerNameText;
        [SerializeField]
        Image colorImage;

        /// <summary>
        /// Method, which draws the data from <c>MultiplayerGameManager</c> and <c>DeathmatchGameManager</c> singletons and assigns it to proper UI elements
        /// </summary>
        /// <param name="playerId">Id of player, which data will be displayed</param>
        public void SetPlayerData(int playerId)
        {
            PlayerData playerData = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(playerId);

            int score = DeathmatchGameManager.instance.GetPlayersKillCount()[playerId];
            string playerName = playerData.playerName.ToString();
            Color color = MultiplayerGameManager.instance.GetPlayerColor(playerData.colorId);

            scoreText.text = score.ToString();
            playerNameText.text = playerName;
            colorImage.color = color;
        }
    }
}