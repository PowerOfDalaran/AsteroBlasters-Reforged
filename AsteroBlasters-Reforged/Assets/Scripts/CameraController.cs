using Cinemachine;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    /// <summary>
    /// Method setting the virtual camera parameter "Follow" to given transform.
    /// </summary>
    /// <param name="transform">Transform of player, the camera have to follow.</param>
    public void FollowPlayer(Transform transform)
    {
        cinemachineVirtualCamera.Follow = transform;
    }
}
