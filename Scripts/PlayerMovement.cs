using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player movement")]
    [Range(1.0f,100.0f)]
    public float playerSpeed; //Annoyingly, the oil script uses this vaariable - will need to be properly encapsulated before release
    [JsonIgnore]
    [SerializeField] Rigidbody2D rb;
    [JsonIgnore]
    Animator anim;
    //bool running = false;

    [Header("Dashing mechanic")]
    [SerializeField] bool isDashing = false;
    [SerializeField] float dashPower;
    Vector3 dashForce;
    [Range(1f,5f)]
    [SerializeField] float dashCooldown;
    [SerializeField] float dashTimer;

    [Header("Camera settings")]
    [Range(0f, 1f)]
    [SerializeField] float screenPercentage; //controls how tightly one can view their player on screen
    [Range(0f,1f)]
    [SerializeField] float deadzone;
    Vector3 newCameraTarget;
    [SerializeField] float cameraSpeed, gamepadSpeed;
    [SerializeField] Camera main_Camera;
    float cameraWidth, cameraHeight;
    [SerializeField] float offsetMultiplier = 0.9f;
    float centreX, centreY,x,y;
    Vector2 virtualAimOffset;

    void Start()
    {
        anim = GetComponent<Animator>();
        main_Camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        main_Camera.transform.LookAt(rb.gameObject.transform);
        //For orthographic cameras if calculating manually  
        cameraHeight = Camera.main.orthographicSize * 2f;  // Total height of the camera's view
        cameraWidth = cameraHeight * Camera.main.aspect;   // Total width of the camera's view based on the aspect ratio
        Cursor.lockState = CursorLockMode.Confined;
        dashTimer = dashCooldown;
        centreX = Screen.width / 2f;
        centreY = Screen.height / 2f;
    }

    void Update()
    {
        //Neither the player nor enemies should be moving during dialogue - should do the same for cutscenes
        if(!DialogueManager.Instance.isDialogueActive)
        {
            // Read the processed input from the manager
            Vector2 moveInput = InputManager.Instance.MovementInput;
            //Get player movement
            anim.SetBool("run", moveInput.magnitude != 0);
            if(dashTimer > 0) dashTimer -= Time.deltaTime;
        }
        
        //Implement player dash
       if(Input.GetButtonDown("Jump") && dashTimer <= 0){
            //We need to get a cooldown and a layer for projectiles
                //Shouldn't avoid damage from projectiles if dashing
            isDashing = true;
            dashForce = rb.velocity * dashPower;
            dashTimer = dashCooldown;
        }
        
        // Calculate new camera target position
        Vector3 playerTransform = gameObject.transform.position;
        float x = virtualAimOffset.x, y = virtualAimOffset.y; // default: keep last value

        if (InputManager.Instance.IsGamepad)
        {
            Vector2 look = InputManager.Instance.LookInput;
            float magnitude = look.magnitude;

            if (magnitude > deadzone)
            {
                // Rescale so input starts at 0 right past the deadzone, not at deadzone value
                float adjustedMagnitude = (magnitude - deadzone) / (1f - deadzone);
                Vector2 adjustedLook = look.normalized * adjustedMagnitude;

                virtualAimOffset += adjustedLook * gamepadSpeed * Time.deltaTime;
                virtualAimOffset = Vector2.ClampMagnitude(virtualAimOffset, 1f);
            }
            //Lowkey, I think that the camera sticking to a corner feels way more annoying on controller than keyboard and mouse
            else virtualAimOffset = Vector2.MoveTowards(virtualAimOffset, Vector2.zero, gamepadSpeed * Time.deltaTime);
            
            x = virtualAimOffset.x;
            y = virtualAimOffset.y;
        }
        else
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();      
            x = (mousePos.x - centreX) / (Screen.width / 2f);
            y = (mousePos.y - centreY) / (Screen.height / 2f);
            virtualAimOffset = new Vector2(x, y); // keep in sync if they switch devices mid-play
        }

        // Determine offset amount
        Vector3 offset = new Vector3(x, y, 0f) *  offsetMultiplier;
        offset.z = -1f;

        // Set new target to offset
        newCameraTarget = playerTransform  + offset;

        // Clamp the camera position so the player is always in view
        newCameraTarget.x = Mathf.Clamp(newCameraTarget.x, playerTransform.x - cameraWidth * screenPercentage, playerTransform.x + cameraWidth * screenPercentage);
        newCameraTarget.y = Mathf.Clamp(newCameraTarget.y, playerTransform.y - cameraHeight * screenPercentage, playerTransform.y + cameraHeight * screenPercentage);
      
        // Update camera position using Lerp for smooth movement
        main_Camera.transform.position = Vector3.Lerp(main_Camera.transform.position, newCameraTarget, cameraSpeed * Time.deltaTime);
      
    }

    void FixedUpdate(){
        //Moving the player
        if(isDashing){
            rb.velocity = dashForce;
            isDashing = false;
        }
        else rb.velocity = InputManager.Instance.MovementInput * playerSpeed;
    } 
}
