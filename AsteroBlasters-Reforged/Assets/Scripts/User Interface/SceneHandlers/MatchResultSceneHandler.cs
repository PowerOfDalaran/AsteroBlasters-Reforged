using System;
using UnityEngine;
using UnityEngine.UI;
using DataStructure;
using SceneManagment;
using UserInterface;

namespace Archive
{
    /// <summary>
    /// Class responsible for managing buttons and other UI elements of <c>MatchResultScene</c> scene.
    /// </summary>
    public class MatchResultSceneHandler : SceneButtonHandler
    {
        [SerializeField] Text TimeLimitText;
        [SerializeField] Text NumberOfPlayersText;

        [SerializeField]
        Button returnToMenuButton;

        void Awake()
        {
            // Buttons
            returnToMenuButton.onClick.AddListener(() =>
            {
                ChangeButtonsState(false);
                Destroy(MatchData.instance.gameObject);
                LevelManager.instance.LoadScene("MainMenuScene");
            });

            // Texts
            TimeSpan timeSpan = TimeSpan.FromSeconds(float.Parse(MatchData.instance.timeLimit));

            TimeLimitText.text = "Time limit: " + UtilitiesToolbox.GetTimeAsString(timeSpan);
            NumberOfPlayersText.text = "Number of players: " + MatchData.instance.numberOfPlayers.ToString();
        }
    }
}
