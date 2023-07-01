using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton responsible for changing scenes. It can be reached by its static property <c>instance</c>.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    //public enum Scenes
    //{
    //    GameScene,
    //    MainMenuScene,
    //    NetworkGameScene,
    //    NetworkLobbyScene,
    //    NetworkMenuScene,
    //}

    private void Awake()
    {
        // Checking if another instance of this class don't exist yet and deleting itself if that is the case
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Method changing scene to one with given name.
    /// Activates animation, which covers the screen.
    /// <br></br>This method is build for handling scene changing BEFORE starting the multiplayer.
    /// </summary>
    /// <param name="sceneName">Name of the scene to be loaded</param>
    /// <param name="transition">Animator of loading screen (UI element)</param>
    public async void LoadScene(string sceneName, Animator transition)
    {
        transition.SetTrigger("LeaveScene");
        await Task.Delay(2000);


        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        //AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
        asyncOperation.allowSceneActivation = false;

        do
        {
            await Task.Delay(100);
        } while (asyncOperation.progress < 0.9f);

        asyncOperation.allowSceneActivation = true;
    }

    /// <summary>
    /// Method calling the <c>NetworkManager</c> to change scene to one with given name.
    /// Activates animation, which covers the screen.
    /// <br></br>This method is build for handling scene changing AFTER starting the multiplayer.
    /// It should be also used only on the host, as other connected players will also automaticly change scene.
    /// </summary>
    /// <param name="sceneName">Name of the scene to be loaded</param>
    /// <param name="transition">Animator of loading screen (UI element)</param>
    public async void NetworkLoadScene(string sceneName, Animator transition)
    {
        transition.SetTrigger("LeaveScene");
        await Task.Delay(2000);
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
