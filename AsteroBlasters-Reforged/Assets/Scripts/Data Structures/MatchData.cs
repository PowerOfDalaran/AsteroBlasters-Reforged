using System;
using UnityEngine;

namespace DataStructure
{
    /// <summary>
    /// Class being a source of data about the recently played match
    /// </summary>
    public class MatchData : MonoBehaviour
    {
        public static MatchData instance;

        /// <summary>
        /// Nested array, holding data about players.
        /// First object is that player's name (string).
        /// Second object is that player's ranking (int).
        /// Third object is that player's color saved as string (string).
        /// Fourth object is that player's kill count (int).
        /// Fifth object is tht players' death count (int).
        /// </summary>
        object[][] playersData;
        public string timeLimit;
        public int numberOfPlayers;
        public bool isDraw;

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
        public void SetData(object[][] PlayersData, string TimeLimit, bool IsDraw)
        {
            playersData = PlayersData;
            timeLimit = TimeLimit;
            numberOfPlayers = PlayersData.Length;
            isDraw = IsDraw;
        }

        /// <summary>
        /// Method returning the player with given position from playersData array
        /// </summary>
        /// <param name="position">Position the player took in the match</param>
        /// 
        /// <returns>Array of player data (null if player wasn't found).
        /// First object is that player's name (string).
        /// Second object is that player's ranking (int).
        /// Third object is that player's color saved as string (string).
        /// Fourth object is that player's kill count (int).
        /// Fifth object is tht players' death count (int).
        /// </returns>
        public object[] GetPlayerOnPosition(int position)
        {
            object[] resultArray = new object[5];

            for (int i = 0; i < playersData.Length; i++)
            {
                if (Convert.ToInt32(playersData[i][1]) == position)
                {
                    resultArray[0] = Convert.ToString(playersData[i][0]);
                    resultArray[1] = Convert.ToInt32(playersData[i][1]);
                    resultArray[2] = Convert.ToString(playersData[i][2]);
                    resultArray[3] = Convert.ToInt32(playersData[i][3]);
                    resultArray[4] = Convert.ToInt32(playersData[i][4]);

                    return resultArray;
                }
            }

            return null;
        }
    }
}
