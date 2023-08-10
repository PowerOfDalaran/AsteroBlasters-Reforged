using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MatchResultSceneHandler : SceneButtonHandler
{
    [SerializeField]
    Text TimeLimitText;
    [SerializeField]
    Text NumberOfPlayersText;

    [SerializeField]
    Button returnToMenuButton;

    void Awake()
    {
        returnToMenuButton.onClick.AddListener(() =>
        {
            ChangeButtonsState(false);
            Destroy(MatchData.instance.gameObject);
            LevelManager.instance.LoadScene("MainMenuScene");
        });

        TimeSpan timeSpan = TimeSpan.FromSeconds(float.Parse(MatchData.instance.timeLimit));

        TimeLimitText.text = "Time limit: " + UtilitiesToolbox.GetTimeAsString(timeSpan);
        NumberOfPlayersText.text = "Number of players: " + MatchData.instance.numberOfPlayers.ToString();
    }
}
