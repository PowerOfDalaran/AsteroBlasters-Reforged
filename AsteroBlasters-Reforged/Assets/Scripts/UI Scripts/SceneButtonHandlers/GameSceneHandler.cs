using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for managing buttons and other UI elements of "GameScene" scene.
/// </summary>
public class GameSceneHandler : SceneButtonHandler
{
    // Buttons
    [SerializeField]
    Button reloadSceneButton;
    [SerializeField]
    Button returnToMenuButton;

    // Other UI elements
    [SerializeField]
    GameObject LoadingScreen;

    private void Awake()
    {
        // Adding functionality to the buttons
        reloadSceneButton.onClick.AddListener(() =>
        {
            ChangeButtonsState(false);
            LevelManager.instance.LoadScene("GameScene");
        });

        returnToMenuButton.onClick.AddListener(() =>
        {
            ChangeButtonsState(false);
            LevelManager.instance.LoadScene("MainMenuScene");
        });
    }
}
