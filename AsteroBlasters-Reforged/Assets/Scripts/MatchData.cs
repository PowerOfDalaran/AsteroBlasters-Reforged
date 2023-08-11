using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class being a source of data about the recently played match
/// </summary>
public class MatchData : MonoBehaviour
{
    public static MatchData instance;

    object[][] playersData;
    public string timeLimit;
    public int numberOfPlayers;

    void Awake()
    {
        // Assigning this object to its static reference and adding it to DontDestroyOnLoad (it will be removed manually)
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Method assigning the given data to object properties
    /// </summary>
    /// <param name="PlayersData">Special array containing the data of the players</param>
    /// <param name="TimeLimit">Starting time for the game</param>
    public void SetData(object[][] PlayersData, string TimeLimit)
    {
        playersData = PlayersData;
        timeLimit = TimeLimit;
        numberOfPlayers = PlayersData.Length;
    }

    /// <summary>
    /// Method returning the player with given position from playersData array
    /// </summary>
    /// <param name="position">Position the player took in the match</param>
    /// <returns>Array of player data (null if player wasn't found)</returns>
    public object[] GetPlayerOnPosition(int position)
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
