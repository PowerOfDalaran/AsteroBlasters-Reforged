using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for managing buttons and other UI elements of "MainMenuScene" scene.
/// </summary>
public class MainMenuSceneHandler : SceneButtonHandler
{
    // Buttons
    [SerializeField]
    Button singleplayerGameButton;
    [SerializeField]
    Button multiplayerGameButton;

    private void Awake()
    {
        // Adding functionality to the buttons
        singleplayerGameButton.onClick.AddListener(() =>
        {
            ChangeButtonsState(false);
            LevelManager.instance.LoadScene("GameScene");
        });

        multiplayerGameButton.onClick.AddListener(() => 
        {
            ChangeButtonsState(false);
            LevelManager.instance.LoadScene("NetworkMenuScene");
        });
    }
}
