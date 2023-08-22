using DataStructure;
using UnityEngine.UI;
using UnityEngine;
using System;
using SceneManagment;

namespace UserInterface
{
    public class EndGameSceneHandler : SceneButtonHandler
    {
        // Buttons
        [SerializeField] Button returnButton;
        [SerializeField]  Button playAgainButton;

        // Other UI elements
        [SerializeField]  Text winnerNameText;
        [SerializeField] GameObject winnerImage;

        [SerializeField] GameObject scoreboardPanel;
        [SerializeField] GameObject[] playerScoresPositions;
        [SerializeField] GameObject playerScoreUIPrefab;

        private void Awake()
        {
            // Setting up the texts
            winnerNameText.text = MatchData.instance.GetPlayerOnPosition(0)[0].ToString();

            //Changing the player visual color
            string winnerColorAsString = MatchData.instance.GetPlayerOnPosition(0)[2].ToString();
            Color winnerColor = UtilitiesToolbox.GetColorFromString(winnerColorAsString);

            winnerImage.GetComponent<Image>().color = winnerColor;

            // Adding functionalities to the buttons
            returnButton.onClick.AddListener(() =>
            {
                ChangeButtonsState(false);
                Destroy(MatchData.instance.gameObject);
                LevelManager.instance.LoadScene("NetworkMenuScene");
            });

            //Creating score board
            for (int i = 0; i < MatchData.instance.numberOfPlayers; i++)
            {
                object[] currentPlayerData = MatchData.instance.GetPlayerOnPosition(i);

                Color playerColor = UtilitiesToolbox.GetColorFromString(currentPlayerData[2].ToString());
                string playerName = currentPlayerData[0].ToString();
                int playerKills = Convert.ToInt32(currentPlayerData[3]); 
                int playerDeaths = Convert.ToInt32(currentPlayerData[4]);

                GameObject newPlayerScore = Instantiate(playerScoreUIPrefab);
                newPlayerScore.GetComponent<PlayerScoreEndGame>().SetPlayerScoreData(playerColor, playerName, playerKills, playerDeaths, i == 0);

                newPlayerScore.transform.SetParent(scoreboardPanel.transform);
                newPlayerScore.transform.position = playerScoresPositions[i].transform.position;
            }
        }
    }

}