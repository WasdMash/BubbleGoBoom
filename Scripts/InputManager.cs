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
    //[SerializeField] private InputActionReference openMenuAction;

    // Public properties to read current movement values from inputSystem
    public Vector2 MovementInput => inputs.Gameplay.Move.ReadValue<Vector2>();
    public Vector2 LookInput => inputs.Gameplay.CameraMove.ReadValue<Vector2>();

    

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
        inputs.Gameplay.EquipItem.performed += FindObjectOfType<Inventory>().useEquippedItem;
        #if UNITY_EDITOR
        inputs.Gameplay.SaveGame.performed += FindObjectOfType<SaveStateManager>().SaveState;
        inputs.Gameplay.LoadGame.performed += FindObjectOfType<SaveStateManager>().LoadState;
        #endif
    }


    private void OnEnable() {
        inputs.Enable();
    }
    //Memory management of input system when script gets disabled
    private void OnDisable()
    {
        inputs.Disable();
    }
    
    private void OnDestroy() => inputs.Disable(); // Good practice to disable on destroy
}
