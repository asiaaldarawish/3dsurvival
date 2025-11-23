using UnityEngine;
using System;

public class HotbarUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private HotbarSlotUI[] hotbarSlots; // size 6
    [SerializeField] private RectTransform highlight;

    public int currentIndex = 0;
    public static event Action<int> OnHotbarChanged;

    private void OnEnable()
    {
        if (inputReader != null)
            inputReader.OnHotbarSelect += SelectSlot;

        InventoryManager.OnInventoryChanged += RefreshHotbar;
    }

    private void OnDisable()
    {
        if (inputReader != null)
            inputReader.OnHotbarSelect -= SelectSlot;

        InventoryManager.OnInventoryChanged -= RefreshHotbar;
    }

    private void Start()
    {
        RefreshHotbar();
        UpdateHighlight();
        OnHotbarChanged?.Invoke(currentIndex);
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length) return;

        currentIndex = index;
        UpdateHighlight();
        OnHotbarChanged?.Invoke(index);
    }

    public void RefreshHotbar()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            var slotData = inventory.slots[i];
            if (slotData.IsEmpty)
                hotbarSlots[i].SetItem(null);
            else
                hotbarSlots[i].SetItem(slotData.item);
        }

        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        if (highlight != null && currentIndex >= 0 && currentIndex < hotbarSlots.Length)
        {
            highlight.position = hotbarSlots[currentIndex].transform.position;
        }
    }
}
