using UnityEngine;
using UnityEngine.UI;
using DataStructure;

namespace UserInterface
{
    /// <summary>
    /// Class attatched to UI podium element, responsible for assigning the proper data to its texts, etc.
    /// </summary>
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
            // Checking if the podium element should be even visible
            if ((position + 1) > MatchData.instance.numberOfPlayers)
            {
                gameObject.SetActive(false);
                return;
            }

            // Getting the data from MatchData instance and assigning it to texts
            object[] playerData = MatchData.instance.GetPlayerOnPosition(position);

            PlayerNameText.text = playerData[0].ToString();
            playerImage.color = UtilitiesToolbox.GetColorFromString(playerData[1].ToString());
        }
    }
}
