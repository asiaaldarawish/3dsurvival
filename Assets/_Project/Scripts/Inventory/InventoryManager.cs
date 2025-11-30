using UnityEngine;
using System;

public static class InventoryEvents
{
    public static Action<ItemData, int> OnItemCollected;
}

public class InventoryManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private int inventorySize = 16;
    public InventoryModel Model { get; private set; }

    public InventorySlot[] slots => Model.Slots;

    [Header("Input")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCamera playerCamera;

    public static event Action OnInventoryChanged;

    private bool isOpen = false;

    private void Awake()
    {
        Model = new InventoryModel(inventorySize);

        if (playerMovement == null)
            playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerCamera == null)
            playerCamera = FindFirstObjectByType<PlayerCamera>();

        // Subscribe to model events
        Model.OnSlotChanged += HandleSlotChanged;
        Model.OnInventoryChanged += HandleInventoryChanged;

        // initial UI paint
        if (inventoryUIManager != null)
            inventoryUIManager.Initialize(this);
    }

    private void OnEnable()
    {
        InventoryEvents.OnItemCollected += HandleItemCollected;
        InventoryUIEvents.OnSlotSwap += HandleSlotSwap;

        if (inputReader != null)
            inputReader.OnInventoryToggle += ToggleInventory;
    }

    private void OnDisable()
    {
        InventoryEvents.OnItemCollected -= HandleItemCollected;
        InventoryUIEvents.OnSlotSwap -= HandleSlotSwap;

        if (inputReader != null)
            inputReader.OnInventoryToggle -= ToggleInventory;
    }

    private void ToggleInventory()
    {
        if (isOpen)
            CloseInventoryPanel();
        else
            OpenInventoryPanel();
    }


    private void HandleItemCollected(ItemData data, int amount)
    {
        int leftover = Model.AddItem(data, amount);
        if (leftover > 0)
        {
            Debug.Log($"Inventory full, couldn't add all of {data.displayName}. Left: {leftover}");
        }
    }

    private void HandleSlotSwap(int from, int to)
    {
        Model.SwapSlots(from, to);
    }

    private void HandleSlotChanged(int index)
    {
        inventoryUIManager?.UpdateSlotUI(index);
    }

    private void HandleInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    public void SplitStack(int index)
    {
        if (!Model.SplitStack(index))
        {
            Debug.Log("No space to split stack.");
        }
    }


    public bool IsOpen() => isOpen;

    public void OpenInventoryPanel()
    {
        if (isOpen) return;

        isOpen = true;
        inventoryUIManager?.SetVisible(true);
        inventoryUIManager?.RefreshAllSlots();
        TogglePlayerControl(false);
    }

    public void CloseInventoryPanel()
    {
        if (!isOpen) return;

        isOpen = false;
        inventoryUIManager?.SetVisible(false);
        TogglePlayerControl(true);
    }

    private void TogglePlayerControl(bool enabled)
    {
        if (playerMovement != null)
            playerMovement.EnableMovement(enabled);

        if (playerCamera != null)
            playerCamera.SetLookEnabled(enabled);
    }
}
