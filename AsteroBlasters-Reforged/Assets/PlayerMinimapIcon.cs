using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Class updating the player character minimap icon color on the game start
/// </summary>
public class PlayerMinimapIcon : NetworkBehaviour
{
    void Start()
    {
        // Checking if this player character is owned by this instance of the game,
        // if not - coloring the icon to the red color
        if (!IsOwner)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
