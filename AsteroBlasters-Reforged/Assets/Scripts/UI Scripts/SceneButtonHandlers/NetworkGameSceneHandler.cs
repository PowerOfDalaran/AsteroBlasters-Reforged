using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
        DeathmatchGameManager.instance.OnPlayersKillCountNetworkListChanged += NetworkGameSceneHandler_OnPlayersKillCountNetworkListChanged;

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

        string minutesLeft = timerTimeSpan.Minutes < 10 ? "0" + timerTimeSpan.Minutes.ToString() : timerTimeSpan.Minutes.ToString();
        string secondsLeft = timerTimeSpan.Seconds < 10 ? "0" + timerTimeSpan.Seconds.ToString() : timerTimeSpan.Seconds.ToString();

        timerText.text = minutesLeft + ":" + secondsLeft;
    }

    /// <summary>
    /// Method being called everytime the playersKillCount list changes.
    /// It runs trough the order list and calls <c>CreatePlayerScoreUI</c> with proper position and id.
    /// </summary>
    private void UpdateScoreBoard()
    {
        int[] newPlayersOrder = OrderThePlayers();

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

    /// <summary>
    /// Method ordering the players by their score.
    /// </summary>
    /// <returns>The array, in which every index corresponds to player index and value is equal to their correct order.</returns>
    private int[] OrderThePlayers()
    {
        List<int> newList = DeathmatchGameManager.instance.GetPlayersKillCount();
        int[] orderedPlayers = new int[newList.Count];

        for (int i = 0; i < newList.Count; i++)
        {
            int highestValue = newList.Max();
            int highestValueIndex = newList.IndexOf(highestValue);

            orderedPlayers[highestValueIndex] = i;
            newList[highestValueIndex] = -1;
        }

        return orderedPlayers;
    }
}
