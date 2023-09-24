using NetworkFunctionality;
using Unity.Netcode;
using UnityEngine;

public class NetworkMinimapCameraController : MonoBehaviour
{
    [SerializeField] Transform playerTransform;

    private void Start()
    {
        playerTransform = MultiplayerGameManager.instance.ownedPlayerCharacter.transform;
        Debug.Log(playerTransform.gameObject.name);
    }

    private void LateUpdate()
    {
        Vector3 newPosition = playerTransform.position;
        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }
}
