using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
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
        if ((position + 1) > MatchData.instance.numberOfPlayers)
        {
            gameObject.SetActive(false);
            return;
        }

        // Getting the data from some kind of object and assigning it to UI elements
        object[] playerData = MatchData.instance.GetPlayerOnPosition(position);

        PlayerNameText.text = playerData[0].ToString();
        playerImage.color = ColorConvertTools.GetColorFromString(playerData[1].ToString());
    }
}
