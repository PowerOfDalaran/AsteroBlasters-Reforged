using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class responsible for controlling the player character, by moving it, activating sound effects, animations etc.
/// </summary>
public class PlayerController : MonoBehaviour
{
    Rigidbody2D myRigidbody2D;
    PlayerControls myPlayerControls;
    Weapon myWeapon;

    [SerializeField]
    float movementSpeed = 3f;
    [SerializeField]
    float rotationSpeed = 720;

    void Awake()
    {
        // Assigning values to class properties
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myPlayerControls = new PlayerControls();
        myWeapon = GetComponent<Weapon>();

        // Adding methods to PlayerControls delegates and activating it
        myPlayerControls.PlayerActions.Shoot.performed += Shoot;
        myPlayerControls.Enable();
    }

    private void FixedUpdate()
    {
        // Reading current input value for movement and adding equivalent force (in case of no input - value of zero)
        Vector2 movementVector = myPlayerControls.PlayerActions.Move.ReadValue<Vector2>();
        Movement(movementVector);
        Rotate(movementVector);
    }

    /// <summary>
    /// Method activating the current weapon in order to fire.
    /// Is added to the "PlayerActions.Move.performed" delegate.
    /// </summary>
    /// <param name="context">Value gathered by input system</param>
    void Shoot(InputAction.CallbackContext context)
    {
        myWeapon.Shoot();
    }

    /// <summary>
    /// Method moving player character by adding force to its rigidbody2D component.
    /// Is triggered in "FixedUpdate()" method each frame.
    /// </summary>
    /// <param name="context">Value gathered by input system</param>
    void Movement(Vector2 movementVector)
    {
        myRigidbody2D.AddForce(movementVector * movementSpeed, ForceMode2D.Force);
    }

    /// <summary>
    /// Method rotating player character by creating new desired rotation and then using it to calculate rotation.
    /// Is triggered in "FixedUpdate()" method each frame.
    /// Not that proud of the result, may look for better rotation system later.
    /// </summary>
    /// <param name="movementVector">Value gathered by input system</param>
    void Rotate(Vector2 movementVector)
    {
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, movementVector);
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
        gameObject.transform.rotation = newRotation;
    }
}
