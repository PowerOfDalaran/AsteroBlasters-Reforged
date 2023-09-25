using NetworkFunctionality;
using UnityEngine;

/// <summary>
/// Class responsible for attatching and updating the minimap camera to the player (multiplayer version)
/// </summary>
public class NetworkMinimapCameraController : MonoBehaviour
{
    [SerializeField] Transform playerTransform;

    private void Start()
    {
        // Accessing the transform of player character owned by this instance of the game
        playerTransform = MultiplayerGameManager.instance.ownedPlayerCharacter.transform;
    }

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            // Updating the camera position to match the player's one (except z parameter)
            Vector3 newPosition = playerTransform.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }
}
