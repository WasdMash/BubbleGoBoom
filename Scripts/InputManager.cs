using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

//Lowkey, I need to figure out which inputs exactly need to be buffered
    //Movement is based on axes - don't really need much real-time action like jumping to be smooth
public class InputManager : MonoBehaviour
{
// Singleton instance
    public static InputManager Instance { get; private set; }
    [Header("Events")]
    private PlayerInputsActions inputs;
    public InputAction fireAction => inputs.Gameplay.Firing;

    // Public properties to read current movement values from inputSystem
    public Vector2 MovementInput => inputs.Gameplay.Move.ReadValue<Vector2>();
    public Vector2 LookInput => inputs.Gameplay.CameraMove.ReadValue<Vector2>();

    enum AimDevice { None, Mouse, Gamepad }
    AimDevice lastDevice = AimDevice.None;
    public bool IsGamepad => lastDevice == AimDevice.Gamepad;
    
    public event Action<InputAction.CallbackContext> OnLookPerformed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // Optional, if manager persists scenes
        inputs = new PlayerInputsActions();
        inputs.Gameplay.CameraMove.performed += ctx =>
        {
            lastDevice = ctx.control.device is Gamepad ? AimDevice.Gamepad : AimDevice.Mouse;
            OnLookPerformed?.Invoke(ctx);
        };
        inputs.Gameplay.EquipItem.performed += FindObjectOfType<Inventory>().useEquippedItem;
        #if UNITY_EDITOR
        inputs.Gameplay.SaveGame.performed += FindObjectOfType<SaveStateManager>().SaveState;
        inputs.Gameplay.LoadGame.performed += FindObjectOfType<SaveStateManager>().LoadState;
        #endif
    }

    private void OnEnable() => inputs.Enable();
    private void OnDisable() => inputs.Disable();
    private void OnDestroy() => inputs.Disable();
}
