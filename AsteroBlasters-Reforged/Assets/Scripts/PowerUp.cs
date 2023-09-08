using PlayerFunctionality;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

        if (playerController != null && collision is not BoxCollider2D)
        {
            BuffPlayer(playerController);
        }
    }

    protected virtual void BuffPlayer(PlayerController playerController)
    {
        // Implement in child classes
    }
}
