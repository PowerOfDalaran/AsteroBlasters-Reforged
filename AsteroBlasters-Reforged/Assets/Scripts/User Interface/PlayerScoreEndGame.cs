using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreEndGame : MonoBehaviour
{
    [SerializeField] Image playerColor;
    [SerializeField] Text playerName;
    [SerializeField] Text playerKills;
    [SerializeField] Text playerDeaths;
    [SerializeField] Image winnerStar;

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
