using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchData : MonoBehaviour
{
    public static MatchData instance;

    object[][] playersData;
    public string timeLimit;
    public int numberOfPlayers;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetData(object[][] PlayersData, string TimeLimit)
    {
        playersData = PlayersData;
        timeLimit = TimeLimit;
        numberOfPlayers = PlayersData.Length;
    }

    public object[]? GetPlayerOnPosition(int position)
    {
        object[] resultArray = new object[2];

        for (int i = 0; i < playersData.Length; i++)
        {
            if (Convert.ToInt32(playersData[i][1]) == position)
            {
                resultArray[0] = Convert.ToString(playersData[i][0]);
                resultArray[1] = Convert.ToString(playersData[i][2]);

                return resultArray;
            }
        }

        return null;
    }
}
