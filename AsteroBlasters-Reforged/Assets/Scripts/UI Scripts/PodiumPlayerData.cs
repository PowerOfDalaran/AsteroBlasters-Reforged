using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class PodiumPlayerData : MonoBehaviour
{
    [SerializeField]
    int position;

    [SerializeField]
    Image playerImage;
    [SerializeField]
    Text PlayerNameText;

    void Awake()
    {
        // Getting the data from some kind of object and assigning it to UI elements
        object[] playerData = MatchData.instance.GetPlayerOnPosition(position);

        PlayerNameText.text = playerData[0].ToString();

        UnityEngine.Color color;
        ColorUtility.TryParseHtmlString(playerData[1].ToString(), out color);
        playerImage.color = color;
    }
}
