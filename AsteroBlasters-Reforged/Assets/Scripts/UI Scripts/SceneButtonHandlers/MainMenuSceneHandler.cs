using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for managing buttons and other UI elements of "MainMenuScene" scene.
/// </summary>
public class MainMenuSceneHandler : MonoBehaviour
{
    // Buttons
    [SerializeField]
    Button[] buttons;

    [SerializeField]
    Button singleplayerGameButton;
    [SerializeField]
    Button multiplayerGameButton;

    // Other UI elements
    [SerializeField]
    GameObject LoadingScreen;

    private void Awake()
    {
        // Adding functionality to the buttons
        singleplayerGameButton.onClick.AddListener(() =>
        {
            TurnOffButtons();
            Animator transition = LoadingScreen.GetComponent<Animator>();
            LevelManager.instance.LoadScene("GameScene", transition);
        });

        multiplayerGameButton.onClick.AddListener(() => 
        {
            TurnOffButtons();
            Animator transition = LoadingScreen.GetComponent<Animator>();
            LevelManager.instance.LoadScene("NetworkMenuScene", transition);
        });
    }

    private void TurnOffButtons()
    {
        foreach (Button button in buttons) 
        {
            button.enabled = false;
        }
    }
}
