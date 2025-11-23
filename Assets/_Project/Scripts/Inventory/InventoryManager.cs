using UnityEngine;
using System;

public static class InventoryEvents
{
    public static Action<ItemData, int> OnItemCollected;

}

public class InventoryManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private InventorySlotUI[] uiSlots;

    [Header("Data")]
    [SerializeField] private int inventorySize = 16;
    public InventoryModel Model { get; private set; }

    // For backward compatibility with other scripts using inventory.slots
    public InventorySlot[] slots => Model.Slots;

    [Header("Input")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private GameObject inventoryUI;

    public static event Action OnInventoryChanged;

    private bool isOpen = false;

    private void Awake()
    {
        Model = new InventoryModel(inventorySize);

        // wire UI slots once
        for (int i = 0; i < uiSlots.Length; i++)
        {
            uiSlots[i].SlotIndex = i;
            uiSlots[i].inventory = this;
            uiSlots[i].Clear();
        }

        if (inventoryUI != null)
            inventoryUI.SetActive(false);

        // subscribe to model events
        Model.OnSlotChanged += HandleSlotChanged;
        Model.OnInventoryChanged += HandleInventoryChanged;

        // initial paint
        RefreshAllSlots();
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

        // optional: unsubscribe model events if you ever recreate Model
        Model.OnSlotChanged -= HandleSlotChanged;
        Model.OnInventoryChanged -= HandleInventoryChanged;
    }

    
    private void ToggleInventory()
    {
        isOpen = !isOpen;

        if (inventoryUI != null)
            inventoryUI.SetActive(isOpen);

        if (isOpen)
            RefreshAllSlots(); // just in case something changed while closed
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
        UpdateSlotUI(index);
    }

    private void HandleInventoryChanged()
    {
        // forward to other systems (Hotbar, OnHand, etc.)
        OnInventoryChanged?.Invoke();
    }

    
    public void RefreshAllSlots()
    {
        for (int i = 0; i < uiSlots.Length; i++)
            UpdateSlotUI(i);

        // also notify others that a full refresh happened
        HandleInventoryChanged();
    }

    private void UpdateSlotUI(int index)
    {
        if (index < 0 || index >= uiSlots.Length) return;
        uiSlots[index].SetItem(Model.Slots[index].item);
    }

    public void SplitStack(int index)
    {
        if (Model.SplitStack(index))
            return;

        Debug.Log("No space to split stack.");
    }


}
