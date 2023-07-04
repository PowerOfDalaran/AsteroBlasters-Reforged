using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for managing buttons and other UI elements of "GameScene" scene.
/// </summary>
public class GameSceneHandler : MonoBehaviour
{
    // Buttons
    [SerializeField]
    Button[] buttons;

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
            TurnOffButtons();
            Animator transition = LoadingScreen.GetComponent<Animator>();
            LevelManager.instance.LoadScene("GameScene", transition);
        });

        returnToMenuButton.onClick.AddListener(() =>
        {
            TurnOffButtons();
            Animator transition = LoadingScreen.GetComponent<Animator>();
            LevelManager.instance.LoadScene("MainMenuScene", transition);
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
