using UnityEngine;

public class HotbarSlotUI : MonoBehaviour
{
    [HideInInspector] public int slotIndex;
    [HideInInspector] public InventoryManager inventory;

    public void Clear()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    public void SetMaterial(InventoryItem item)
    {
        var obj = Instantiate(inventory.materialSlotPrefab, transform);
        obj.GetComponent<MaterialSlotUI>().Set(item.data as MaterialData, item.count);
    }

    public void SetTool(InventoryItem item)
    {
        var obj = Instantiate(inventory.toolSlotPrefab, transform);
        obj.GetComponent<ToolSlotUI>().Set(item.data as ToolData, item.durability);
    }
}
