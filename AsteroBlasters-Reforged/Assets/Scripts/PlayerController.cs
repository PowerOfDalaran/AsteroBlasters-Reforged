using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class responsible for controlling the player character, by moving it, activating sound effects, animations etc.
/// </summary>
public class PlayerController : MonoBehaviour
{
    Rigidbody2D myRigidbody2D;
    PlayerControls myPlayerControls;

    void Awake()
    {
        // Assigning values to class properties
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myPlayerControls = new PlayerControls();

        // Adding methods to PlayerControls delegates and activating it
        myPlayerControls.PlayerActions.Move.performed += Movement;
        myPlayerControls.PlayerActions.Shoot.performed += Shoot;
        myPlayerControls.Enable();
    }

    /// <summary>
    /// Method activating the current weapon in order to fire (curently in development).
    /// Is added to the "PlayerActions.Move.performed" delegate.
    /// </summary>
    /// <param name="context">Value gathered by input system</param>
    void Shoot(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<float>());
    }

    /// <summary>
    /// Method moving player character by adding force to its rigidbody2D component (currently in development).
    /// Is added to the "PlayerActions.Shoot.performed" delegate.
    /// </summary>
    /// <param name="context">Value gathered by input system</param>
    void Movement(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
    }
}
