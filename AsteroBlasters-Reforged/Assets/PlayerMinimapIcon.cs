using Unity.Netcode;
using UnityEngine;

public class PlayerMinimapIcon : NetworkBehaviour
{
    void Start()
    {
        if (!IsOwner)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
