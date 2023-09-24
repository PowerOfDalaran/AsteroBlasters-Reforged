using UnityEngine;

/// <summary>
/// Class responsible for attatching and updating the minimap camera to the player (singleplyer version)
/// </summary>
public class MinimapCameraController : MonoBehaviour
{
    Transform playerTransform;

    private void LateUpdate()
    {
        // Updating the camera position to match the player's one (except z parameter)
        Vector3 newPosition = playerTransform.position;
        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }
}
