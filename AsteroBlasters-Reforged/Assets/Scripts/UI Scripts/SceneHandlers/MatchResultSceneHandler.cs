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

        NetworkManager.Singleton.Shutdown();

        Destroy(MultiplayerGameManager.instance.gameObject);
        Destroy(LobbyManager.instance.gameObject);
        Destroy(NetworkManager.Singleton);

        returnToMenuButton.onClick.AddListener(() =>
        {
            ChangeButtonsState(false);
            Destroy(MatchData.instance.gameObject);
            LevelManager.instance.LoadScene("MainMenuScene");
        });

        TimeLimitText.text = MatchData.instance.timeLimit;
        NumberOfPlayersText.text = MatchData.instance.numberOfPlayers.ToString();
    }
}
