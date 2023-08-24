using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Others
{
    /// <summary>
    /// Simple script allowing to attatch the camera to its owner in multiplayer.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance;
        private CinemachineVirtualCamera cinemachineVirtualCamera;

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

            // Assigning values to properties
            cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += SetCameraConfiner;
        }

        /// <summary>
        /// Method setting the virtual camera parameter "Follow" to given transform.
        /// </summary>
        /// <param name="transform">Transform of player, the camera have to follow.</param>
        public void FollowPlayer(Transform transform)
        {
            cinemachineVirtualCamera.Follow = transform;
        }

        void SetCameraConfiner(Scene scene0, Scene scene1)
        {
            GameObject cameraConfiner = GameObject.FindGameObjectWithTag("CameraConfiner");

            if (cameraConfiner != null)
            {
                gameObject.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraConfiner.GetComponent<PolygonCollider2D>();
            }
        }
    }
}