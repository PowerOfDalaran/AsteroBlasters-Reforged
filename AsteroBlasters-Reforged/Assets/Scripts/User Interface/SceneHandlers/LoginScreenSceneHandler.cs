using NetworkFunctionality;
using UnityEngine;
using UnityEngine.UI;
using Messages;
using SceneManagment;

namespace UserInterface
{
    /// <summary>
    /// Class responsible for managing the buttons and other UI elements of <c>LoginScene</c> scene.
    /// </summary>
    public class LoginScreenSceneHandler : SceneButtonHandler
    {
        // Buttons
        [SerializeField] Button setPlayerNameButton;
        [SerializeField] Button returnButton;

        // Other UI elements
        [SerializeField] InputField playerNameInputField;

        private void Awake()
        {
            // Adding functionality to the buttons
            setPlayerNameButton.onClick.AddListener(() =>
            {
                ChangeButtonsState(false);

                if (playerNameInputField.text.Length > 0)
                {
                    LobbyManager.instance.playerName = playerNameInputField.text;
                    LevelManager.instance.LoadScene("NetworkMenuScene");
                }
                else
                {
                    ChangeButtonsState(true);
                    MessageSystem.instance.AddMessage("Enter the proper name", 2000, MessageSystem.MessagePriority.Medium);
                }
            });

            returnButton.onClick.AddListener(() =>
            {
                ChangeButtonsState(false);
                UtilitiesToolbox.DeleteNetworkConnections(true, false, true, false);
                LevelManager.instance.LoadScene("MainMenuScene");
            });
        }
    }
}
