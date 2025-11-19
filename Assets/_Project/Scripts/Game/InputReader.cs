using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Adapter around the generated PlayerInputActions. Raises events other systems subscribe to.
/// No direct gameplay logic here — only input -> events.
/// </summary>

[DisallowMultipleComponent]
public class InputReader : MonoBehaviour
{
    private PlayerInputActions actions;

    // Movement/look
    public event Action<Vector2> OnMove = delegate { };
    public event Action<Vector2> OnLook = delegate { };

    // Mouse wheel / zoom
    public event Action<float> OnZoom = delegate { };

    // Buttons
    public event Action OnJump = delegate { };
    public event Action<bool> OnSprint = delegate { };
    public event Action OnInteract = delegate { };
    public event Action OnPlace = delegate { };
    public event Action OnAttack = delegate { };
    public event Action OnPause = delegate { };

    public event Action<int> OnHotbarSelect = delegate { };
    public event Action OnInventoryToggle = delegate { };



    private void Awake()
    {
        actions = new PlayerInputActions();

    }

    private void OnEnable()
    {
        actions.Gameplay.Enable();

        // Value callbacks
        actions.Gameplay.Move.performed += HandleMove;
        actions.Gameplay.Move.canceled += HandleMove;

        actions.Gameplay.Look.performed += HandleLook;
        actions.Gameplay.Look.canceled += HandleLook;

        actions.Gameplay.Zoom.performed += HandleZoom;
        actions.Gameplay.Zoom.canceled += HandleZoom;

        // Button callbacks
        actions.Gameplay.Jump.performed += ctx => OnJump();
        actions.Gameplay.Sprint.performed += ctx => OnSprint(true);
        actions.Gameplay.Sprint.canceled += ctx => OnSprint(false);
        actions.Gameplay.Interact.performed += ctx => OnInteract();
        actions.Gameplay.Place.performed += ctx => OnPlace();
        actions.Gameplay.Attack.performed += ctx => OnAttack();
        actions.Gameplay.Pause.performed += ctx => OnPause();
        actions.Gameplay.Inventory.performed += ctx => OnInventoryToggle();



        actions.Gameplay.Hotbar1.performed += ctx => OnHotbarSelect(0);
        actions.Gameplay.Hotbar2.performed += ctx => OnHotbarSelect(1);
        actions.Gameplay.Hotbar3.performed += ctx => OnHotbarSelect(2);
        actions.Gameplay.Hotbar4.performed += ctx => OnHotbarSelect(3);
        actions.Gameplay.Hotbar5.performed += ctx => OnHotbarSelect(4);
        actions.Gameplay.Hotbar6.performed += ctx => OnHotbarSelect(5);

    }

    private void OnDisable()
    {
        // Unsubscribe MOVEMENT/LOOK/ZOOM
        actions.Gameplay.Move.performed -= HandleMove;
        actions.Gameplay.Move.canceled -= HandleMove;

        actions.Gameplay.Look.performed -= HandleLook;
        actions.Gameplay.Look.canceled -= HandleLook;

        actions.Gameplay.Zoom.performed -= HandleZoom;
        actions.Gameplay.Zoom.canceled -= HandleZoom;

        actions.Gameplay.Disable();
    }

    private void HandleMove(InputAction.CallbackContext ctx)
    {
        OnMove(ctx.ReadValue<Vector2>());
    }

    private void HandleLook(InputAction.CallbackContext ctx)
    {
        OnLook(ctx.ReadValue<Vector2>());
    }

    private void HandleZoom(InputAction.CallbackContext ctx)
    {
        OnZoom(ctx.ReadValue<float>());
    }

    public void SetEnabled(bool enabled)
    {
        if (enabled) actions.Gameplay.Enable();
        else actions.Gameplay.Disable();
    }
}
