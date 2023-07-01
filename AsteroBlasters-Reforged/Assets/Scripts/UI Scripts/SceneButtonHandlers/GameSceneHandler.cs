using UnityEngine;
using UnityEngine.UI;

public class GameSceneHandler : MonoBehaviour
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
            Animator transition = LoadingScreen.GetComponent<Animator>();
            LevelManager.instance.LoadScene("GameScene", transition);
        });

        returnToMenuButton.onClick.AddListener(() =>
        {
            Animator transition = LoadingScreen.GetComponent<Animator>();
            LevelManager.instance.LoadScene("MainMenuScene", transition);
        });
    }
}
