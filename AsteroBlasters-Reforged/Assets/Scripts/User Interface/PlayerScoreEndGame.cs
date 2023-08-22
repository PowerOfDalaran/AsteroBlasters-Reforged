using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    /// <summary>
    /// Class attatched to PlayerScoreEndgame prefab, responsible for properly assigning its data to UI elements
    /// </summary>
    public class PlayerScoreEndGame : MonoBehaviour
    {
        [SerializeField] Image playerColor;
        [SerializeField] Text playerName;
        [SerializeField] Text playerKills;
        [SerializeField] Text playerDeaths;
        [SerializeField] Image winnerStar;

        /// <summary>
        /// Method, which assign the given data to UI elements of this object 
        /// </summary>
        /// <param name="PlayerColor">The color of the player, which data should be displayed</param>
        /// <param name="PlayerName">The name of the player, which data should be displayed</param>
        /// <param name="PlayerKills">The number of the player kills, which data should be displayed</param>
        /// <param name="PlayerDeaths">The number of the player deaths, which data should be displayed</param>
        /// <param name="isWinner">Is this player a winner?</param>
        public void SetPlayerScoreData(Color PlayerColor, string PlayerName, int PlayerKills, int PlayerDeaths, bool isWinner)
        {
            playerColor.color = PlayerColor;

            playerName.text = PlayerName;
            playerKills.text = PlayerKills.ToString();
            playerDeaths.text = PlayerDeaths.ToString();

            if (!isWinner)
            {
                winnerStar.gameObject.SetActive(false);
            }
        }
    }
}