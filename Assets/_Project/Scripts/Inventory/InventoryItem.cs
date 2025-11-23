[System.Serializable]
public class InventoryItem
{
    public ItemData data;
    public int count;       // used if stackable
    public int durability;  // used if hasDurability
    
}
