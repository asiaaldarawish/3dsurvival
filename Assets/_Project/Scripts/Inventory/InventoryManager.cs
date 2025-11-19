using UnityEngine;
using System.Collections.Generic;
using System;



public static class InventoryEvents
{
    public static Action<MaterialData> OnMaterialCollected;
    public static Action<ToolData> OnToolCollected;

}


public class InventoryManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private InventorySlotUI[] uiSlots;    // 22 slots in inspector
    [SerializeField] public GameObject materialSlotPrefab;
    [SerializeField] public GameObject toolSlotPrefab;

    [Header("Data")]
    public InventorySlot[] slots = new InventorySlot[22];


    public static event Action OnInventoryChanged;

    [SerializeField] public InputReader inputReader;
    [SerializeField] private GameObject inventoryUI;
    private bool isOpen = false;


    private void Awake()
    {
        // init backend slots
        for (int i = 0; i < slots.Length; i++)
            slots[i] = new InventorySlot();

        // wire UI slots
        for (int i = 0; i < uiSlots.Length; i++)
        {
            uiSlots[i].SlotIndex = i;
            uiSlots[i].inventory = this;
        }

        RefreshUI();
    }

    private void OnEnable()
    {
        inputReader.OnInventoryToggle += ToggleInventory;
        InventoryEvents.OnMaterialCollected += AddMaterial;
        InventoryEvents.OnToolCollected += AddTool;
        InventoryUIEvents.OnSlotSwap += SwapSlots;
    }

    private void OnDisable()
    {
        inputReader.OnInventoryToggle -= ToggleInventory;
        InventoryEvents.OnMaterialCollected -= AddMaterial;
        InventoryEvents.OnToolCollected -= AddTool;
        InventoryUIEvents.OnSlotSwap -= SwapSlots;
    }

    private void AddMaterial(MaterialData data)
    {
        Debug.Log($"Adding material: {data.name}");

        // Try stack with existing same material
        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            if (slot.item != null && slot.item.data == data)
            {
                slot.item.count++;
                RefreshUI();
                return;
            }
        }

        // Otherwise, find an empty slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = new InventoryItem
                {
                    data = data,
                    count = 1
                };

                RefreshUI();
                return;
            }
        }

        Debug.LogWarning("Inventory full, cannot add material.");
    }

    private void AddTool(ToolData data)
    {
        Debug.Log($"Adding tool: {data.name}");

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = new InventoryItem
                {
                    data = data,
                    durability = data.durability
                };

                RefreshUI();
                return;
            }
        }

        Debug.LogWarning("Inventory full, cannot add tool.");
    }

    private void SwapSlots(int from, int to)
    {
        var temp = slots[from].item;
        slots[from].item = slots[to].item;
        slots[to].item = temp;

        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            var ui = uiSlots[i];
            var slot = slots[i];

            ui.Clear();

            if (slot.item == null)
                continue;

            if (slot.item.data is MaterialData)
                ui.SetMaterial(slot.item);
            else if (slot.item.data is ToolData)
                ui.SetTool(slot.item);
        }


        OnInventoryChanged?.Invoke();

    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryUI.SetActive(isOpen);

    }

}
