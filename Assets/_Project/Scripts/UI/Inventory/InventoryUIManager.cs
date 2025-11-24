using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventorySlotUI[] uiSlots;

    private InventoryManager inventory;

    public void Initialize(InventoryManager manager)
    {
        inventory = manager;

        for (int i = 0; i < uiSlots.Length; i++)
        {
            uiSlots[i].SlotIndex = i;
            uiSlots[i].Bind(inventory);
            uiSlots[i].Clear();
        }

        SetVisible(false);
        RefreshAllSlots();
    }

    public void SetVisible(bool visible)
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(visible);

        // unlock mouse when inventory is open
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }


    public void RefreshAllSlots()
    {
        if (inventory == null) return;

        for (int i = 0; i < uiSlots.Length; i++)
        {
            UpdateSlotUI(i);
        }
    }

    public void UpdateSlotUI(int index)
    {
        if (inventory == null) return;
        if (index < 0 || index >= uiSlots.Length) return;
        uiSlots[index].SetItem(inventory.slots[index].item);
    }
}
