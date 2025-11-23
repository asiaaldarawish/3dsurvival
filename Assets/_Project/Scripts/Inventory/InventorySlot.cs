[System.Serializable]
public class InventorySlot
{
    public InventoryItem item;

    public bool IsEmpty => item == null || item.data == null;

    public void Clear()
    {
        item = null;
    }
}
