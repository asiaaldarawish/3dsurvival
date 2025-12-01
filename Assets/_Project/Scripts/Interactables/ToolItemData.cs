using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/Tool")]
public class ToolItemData : ItemData
{
    [Header("Tool Stats")]
    public ToolType toolType = ToolType.None;
    public int maxDurability = 100;
}

public enum ToolType
{
    None,
    Axe,
    Pickaxe,
    Hammer
}