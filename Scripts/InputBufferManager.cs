using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

//Lowkey, I need to figure out which inputs exactly need to be buffered
    //Movement is based on axes - don't really need much real-time action like jumping to be smooth
public class InputBufferManager : MonoBehaviour
{
    // A queue to store buffered inputs
    private Queue<InputAction> inputBuffer = new Queue<InputAction>();
    // A duration for how long an input should be buffered
    public float bufferTime = 0.2f;

    private PlayerInputActions playerInputActions;
    private float lastJumpTime = -Mathf.Infinity;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        // Subscribe to the jump action event
        playerInputActions.Player.Jump.performed += OnJumpPerformed;
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    // This method is called by the Input System when the jump button is pressed
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        // Add the action to the buffer with a timestamp
        inputBuffer.Enqueue(playerInputActions.Player.Jump);
        lastJumpTime = Time.time;
    }

    private void Update()
    {
        // Process the buffer in the Update loop
        ProcessInputBuffer();
    }

    private void ProcessInputBuffer()
    {
        // Dequeue inputs that have expired
        while (inputBuffer.Count > 0 && Time.time - lastJumpTime > bufferTime)
        {
            inputBuffer.Dequeue();
            // Re-evaluate lastJumpTime if more items exist
            if (inputBuffer.Count > 0)
            {
                // Note: For a proper system, you'd store the timestamp *with* the input, 
                // but this basic example uses the single lastJumpTime for simplicity.
                // A more robust system uses a custom struct/class with a timestamp.
            }
        }

        // Example check: if character becomes grounded, check the buffer for a jump
        if (IsGrounded()) // Replace IsGrounded() with your actual ground check logic
        {
            if (inputBuffer.Count > 0)
            {
                // If a jump is buffered, execute it
                Jump();
                inputBuffer.Clear(); // Clear buffer after action
            }
        }
    }

    // Dummy methods for example
    private bool IsGrounded()
    {
        // Implement your physics/ground check logic here
        return false; 
    }

    private void Jump()
    {
        Debug.Log("Jump executed from buffer!");
        // Add your jump logic here
    }
}
