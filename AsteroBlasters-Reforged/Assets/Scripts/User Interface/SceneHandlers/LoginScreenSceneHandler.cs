using NetworkFunctionality;
using UnityEngine;
using UnityEngine.UI;
using Messages;
using SceneManagment;

namespace UserInterface
{
    public class LoginScreenSceneHandler : SceneButtonHandler
    {
        [SerializeField] Button setPlayerNameButton;
        [SerializeField] Button returnButton;

        [SerializeField] InputField playerNameInputField;

        private void Awake()
        {
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
