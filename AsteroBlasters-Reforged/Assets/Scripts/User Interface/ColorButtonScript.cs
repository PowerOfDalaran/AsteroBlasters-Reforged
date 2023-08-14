using UnityEngine;
using UnityEngine.UI;
using NetworkFunctionality;

namespace UserInterface
{
    /// <summary>
    /// Class responsible for managing special buttons, which changes the color of local player
    /// </summary>
    public class ColorButtonScript : MonoBehaviour
    {
        [SerializeField]
        int colorId;
        Image myImage;

        private void Awake()
        {
            // Adding the lambda method to onClick event
            GetComponent<Button>().onClick.AddListener(() =>
            {
                MultiplayerGameManager.instance.ChangePlayerColor(colorId);
            });
        }

        private void Start()
        {
            // Accessing the image component and assigning the proper color to the button
            myImage = GetComponent<Image>();
            myImage.color = MultiplayerGameManager.instance.GetPlayerColor(colorId);
        }
    }
}
