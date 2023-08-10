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
        Destroy(CameraController.instance.gameObject);

        returnToMenuButton.onClick.AddListener(() =>
        {
            Debug.Log("Button clicked");

            ChangeButtonsState(false);
            Destroy(MatchData.instance.gameObject);
            LevelManager.instance.LoadScene("MainMenuScene");
        });

        TimeLimitText.text = "Time limit: " + MatchData.instance.timeLimit;
        NumberOfPlayersText.text = "Number of players: " + MatchData.instance.numberOfPlayers.ToString();
    }
}
