using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    Transform playerTransform;

    private void LateUpdate()
    {
        Vector3 newPosition = playerTransform.position;
        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }
}
