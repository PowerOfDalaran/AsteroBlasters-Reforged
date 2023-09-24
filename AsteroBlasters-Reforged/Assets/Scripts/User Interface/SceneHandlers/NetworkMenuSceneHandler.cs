using UnityEngine;
using UnityEngine.UI;
using SceneManagment;
using NetworkFunctionality;
using Messages;
using System;

namespace UserInterface
{
    /// <summary>
    /// Class responsible for managing buttons and other UI elements of "NetworkMenuScene" scene.
    /// </summary>
    public class NetworkMenuSceneHandler : SceneButtonHandler
    {
        // Buttons
        [SerializeField] Button joinGameButton;
        [SerializeField] Button createGameButton;
        [SerializeField] Button returnToMenuButton;

        // Other UI elements
        [SerializeField] InputField lobbyCodeInputField;

        [SerializeField] Slider maxPlayersSlider;
        [SerializeField] Text maxPlayersText;

        [SerializeField] Slider gameLengthSlider;
        [SerializeField] Text gameLengthText;

        [SerializeField] Slider victoryTresholdSlider;
        [SerializeField] Text victoryTresholdText;

        [SerializeField] InputField lobbyNameInputField;


        private void Awake()
        {
            // Adding functionality to the buttons
            createGameButton.onClick.AddListener(CreateGameButton);
            joinGameButton.onClick.AddListener(JoinGameButton);

            returnToMenuButton.onClick.AddListener(() =>
            {
                // Logging out of services and destroying singletons
                ChangeButtonsState(false);

                UtilitiesToolbox.DeleteNetworkConnections(true, true, true, true);

                LevelManager.instance.LoadScene("LoginScene");
            });

            maxPlayersSlider.onValueChanged.AddListener((float newValue) =>
            {
                maxPlayersText.text = newValue.ToString();
            });

            gameLengthSlider.onValueChanged.AddListener((float newValue) =>
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(newValue);
                string text = UtilitiesToolbox.GetTimeAsString(timeSpan);

                gameLengthText.text = text;
            });

            victoryTresholdSlider.onValueChanged.AddListener((float newValue) =>
            {
                victoryTresholdText.text = newValue.ToString();
            });
        }

        /// <summary>
        /// Method providing functionalities to CreateGame button, it calls <c>LobbyManager</c> to create new lobby with given parameters.
        /// If the attempt succedes, the scene is changed, otherwise error message is displayed.
        /// The method was moved to different class in order to use async properties.
        /// </summary>
        async void CreateGameButton()
        {
            if (lobbyNameInputField.text.Length > 0)
            {
                ChangeButtonsState(false);
                bool creatingResult = await LobbyManager.instance.CreateLobby(lobbyNameInputField.text, (int)maxPlayersSlider.value);

                MultiplayerGameManager.instance.timeLimit.Value = gameLengthSlider.value;
                MultiplayerGameManager.instance.victoryTreshold.Value = (int)victoryTresholdSlider.value;

                if (creatingResult)
                {
                    LevelManager.instance.LoadScene("NetworkLobbyScene");
                }
                else
                {
                    ChangeButtonsState(true);
                    MessageSystem.instance.AddMessage("An error has occurred while creating the lobby!", 3000, MessageSystem.MessagePriority.High);
                }
            }
            else
            {
                MessageSystem.instance.AddMessage("Enter the proper lobby name", 2000, MessageSystem.MessagePriority.Medium);
            }
        }

        /// <summary>
        /// Method providing functionalities to JoinGame button, it calls <c>LobbyManager</c> to connect to lobby with given code.
        /// If the attempt succedes, the scene is changed, otherwise error message is displayed.
        /// The method was moved to different class in order to use async properties.
        /// </summary>
        async void JoinGameButton()
        {
            if (lobbyCodeInputField.text.Length > 0)
            {
                ChangeButtonsState(false);
                bool joiningReslut = await LobbyManager.instance.JoinLobbyByCode(lobbyCodeInputField.text);

                if (joiningReslut)
                {
                    LevelManager.instance.LoadScene("NetworkLobbyScene");
                }
                else
                {
                    ChangeButtonsState(true);
                    MessageSystem.instance.AddMessage("An error has occurred while joining the lobby!", 3000, MessageSystem.MessagePriority.High);
                }
            }
            else
            {
                MessageSystem.instance.AddMessage("Enter the proper lobby code", 2000, MessageSystem.MessagePriority.Medium);
            }
        }
    }
}
