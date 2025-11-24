using System;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int count;
    public int durability;

    public bool IsEmpty => data == null;
}