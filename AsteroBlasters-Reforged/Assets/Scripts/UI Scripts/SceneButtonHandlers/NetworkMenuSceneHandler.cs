using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for managing buttons and other UI elements of "NetworkMenuScene" scene.
/// </summary>
public class NetworkMenuSceneHandler : MonoBehaviour
{
    // Buttons
    [SerializeField]
    Button joinGameButton;
    [SerializeField]
    Button createGameButton;
    [SerializeField]
    Button returnToMenuButton;
    [SerializeField]
    Button setPlayerNameButton;

    // Other UI elements
    [SerializeField]
    InputField lobbyCodeInputField;
    [SerializeField]
    Slider maxPlayersSlider;
    [SerializeField]
    InputField lobbyNameInputField;
    [SerializeField]
    GameObject LoadingScreen;
    [SerializeField]
    InputField playerNameInputField;

    private void Awake()
    {
        // Adding functionality to the buttons
        createGameButton.onClick.AddListener(CreateGameButton);
        joinGameButton.onClick.AddListener(JoinGameButton);

        returnToMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();

            Destroy(NetworkManager.Singleton.gameObject);
            Destroy(LobbyManager.instance.gameObject.gameObject);

            Animator transition = LoadingScreen.GetComponent<Animator>();
            LevelManager.instance.LoadScene("MainMenuScene", transition);
        });

        setPlayerNameButton.onClick.AddListener(() =>
        {
            LobbyManager.instance.playerName = playerNameInputField.text;
        });
    }

    /// <summary>
    /// Method providing functionalities to CreateGame button, it calls <c>LobbyManager</c> to create new lobby with given parameters.
    /// If the attempt succedes, the scene is changed, otherwise error message is displayed.
    /// The method was moved to different class in order to use async properties.
    /// </summary>
    async void CreateGameButton()
    {
        bool creatingResult = await LobbyManager.instance.CreateLobby(lobbyNameInputField.text, (int)maxPlayersSlider.value);

        if (creatingResult)
        {
            Animator transition = LoadingScreen.GetComponent<Animator>();
            LevelManager.instance.LoadScene("NetworkLobbyScene", transition);
        }
        else
        {
            MessageSystem.instance.AddMessage("An error has occurred while creating the lobby!", 3000, MessageSystem.MessagePriority.High);
        }
    }

    /// <summary>
    /// Method providing functionalities to JoinGame button, it calls <c>LobbyManager</c> to connect to lobby with given code.
    /// If the attempt succedes, the scene is changed, otherwise error message is displayed.
    /// The method was moved to different class in order to use async properties.
    /// </summary>
    async void JoinGameButton()
    {
        bool joiningReslut = await LobbyManager.instance.JoinLobbyByCode(lobbyCodeInputField.text);

        if (joiningReslut)
        {
            Animator transition = LoadingScreen.GetComponent<Animator>();
            LevelManager.instance.LoadScene("NetworkLobbyScene", transition);
        }
        else
        {
            MessageSystem.instance.AddMessage("An error has occurred while joining the lobby!", 3000, MessageSystem.MessagePriority.High);
        }
    }
}
