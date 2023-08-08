using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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

    // Start is called before the first frame update
    void Start()
    {
        MultiplayerGameManager.instance.StartTheGame();
        DeathmatchGameManager.instance.OnPlayersKillCountNetworkListChanged += NetworkGameSceneHandler_OnPlayersKillCountNetworkListChanged;

        UpdateScoreBoard();
    }

    private void Update()
    {
        TimeSpan timerTimeSpan = TimeSpan.FromSeconds(DeathmatchGameManager.instance.timeLeft.Value);

        string minutesLeft = timerTimeSpan.Minutes < 10 ? "0" + timerTimeSpan.Minutes.ToString() : timerTimeSpan.Minutes.ToString();
        string secondsLeft = timerTimeSpan.Seconds < 10 ? "0" + timerTimeSpan.Seconds.ToString() : timerTimeSpan.Seconds.ToString();

        timerText.text = minutesLeft + ":" + secondsLeft;
    }

    public void NetworkGameSceneHandler_OnPlayersKillCountNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateScoreBoard();
    }

    private void UpdateScoreBoard()
    {
        for(int i = 0; i < DeathmatchGameManager.instance.playersKillCount.Count; i++)
        {
            GameObject newPlayerScoreUI = Instantiate(playerScoreUI);
            newPlayerScoreUI.transform.SetParent(scoreBoard.transform, false);
            newPlayerScoreUI.transform.position = positions[i].transform.position;
            newPlayerScoreUI.GetComponent<PlayerScore>().SetPlayerData(i);
        }
    }
}
