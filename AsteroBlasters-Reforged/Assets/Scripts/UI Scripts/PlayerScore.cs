using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text playerNameText;
    [SerializeField]
    Image colorImage;

    public void SetPlayerData(int playerId)
    {
        PlayerData playerData = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(playerId);

        int score = DeathmatchGameManager.instance.playersKillCount[playerId];
        string playerName = playerData.playerName.ToString();
        Color color = MultiplayerGameManager.instance.GetPlayerColor(playerData.colorId);

        scoreText.text = score.ToString();
        playerNameText.text = playerName;
        colorImage.color = color;
    }
}
