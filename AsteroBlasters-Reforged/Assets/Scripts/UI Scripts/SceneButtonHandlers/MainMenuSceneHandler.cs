using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSceneHandler : MonoBehaviour
{
    // Buttons
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
            Animator transition = LoadingScreen.GetComponent<Animator>();
            LevelManager.instance.LoadScene("GameScene", transition);
        });

        multiplayerGameButton.onClick.AddListener(() => 
        {
            Animator transition = LoadingScreen.GetComponent<Animator>();
            LevelManager.instance.LoadScene("NetworkMenuScene", transition);
        });
    }
}
