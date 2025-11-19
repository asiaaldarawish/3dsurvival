using UnityEngine;
using System;

public class HotbarUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private InventorySlotUI[] hotbarSlots;
    [SerializeField] private RectTransform highlight;

    public int currentIndex = 0;
    public static event Action<int> OnHotbarChanged;

    private void OnEnable()
    {
        inputReader.OnHotbarSelect += SelectSlot;
        InventoryManager.OnInventoryChanged += RefreshHotbar;
    }

    private void OnDisable()
    {
        inputReader.OnHotbarSelect -= SelectSlot;
        InventoryManager.OnInventoryChanged -= RefreshHotbar;
    }

    private void Start()
    {
        RefreshHotbar();
        UpdateHighlight();
    }

    public void SelectSlot(int index)
    {
        currentIndex = index;
        UpdateHighlight();

        OnHotbarChanged?.Invoke(index);
    }

    public void RefreshHotbar()
    {
        for (int i = 0; i < 6; i++)
        {
            hotbarSlots[i].Clear();

            var slot = inventory.slots[i];
            if (slot.item == null) continue;

            if (slot.item.data is MaterialData)
                hotbarSlots[i].SetMaterial(slot.item);
            else
                hotbarSlots[i].SetTool(slot.item);
        }

        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        highlight.position = hotbarSlots[currentIndex].transform.position;
    }
}
