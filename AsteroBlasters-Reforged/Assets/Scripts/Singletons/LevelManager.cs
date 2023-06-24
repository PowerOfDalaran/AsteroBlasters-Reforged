using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Singleton responsible for changing scenes. It can be reached by its static property <c>instance</c>.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

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
    /// Method changing scene to one with given build index.
    /// Base method, which doesn't use any animator.
    /// </summary>
    /// <param name="sceneId">Build index of the scene to be loaded</param>
    public async void LoadScene(int sceneId)
    {

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneId);
        asyncOperation.allowSceneActivation = false;

        do
        {
            await Task.Delay(100);
        } while (asyncOperation.progress < 0.9f);

        asyncOperation.allowSceneActivation = true;
    }

    /// <summary>
    /// Method changing scene to one with given build index.
    /// Activates animation, which covers the screen.
    /// </summary>
    /// <param name="sceneId">Build index of the scene to be loaded</param>
    /// <param name="transition">Animator of loading screen (UI element)</param>
    public async void LoadScene(int sceneId, Animator transition)
    {

        transition.SetTrigger("LeaveScene");
        await Task.Delay(2000);


        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneId);
        asyncOperation.allowSceneActivation = false;

        do
        {
            await Task.Delay(100);
        } while (asyncOperation.progress < 0.9f);

        asyncOperation.allowSceneActivation = true;
    }
}
