using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string id;           // unique string, e.g. "wood_log", "iron_pickaxe"
    public string displayName;
    [TextArea] public string description;

    [Header("Visuals")]
    public Sprite icon;
    public GameObject handPrefab;   // model when held
    public GameObject worldPrefab;  // pickup prefab

    [Header("Type")]
    public ItemCategory category;

    [Header("Stacking")]
    public bool stackable = true;
    public int maxStack = 99;

    [Header("Durability (for tools / gear)")]
    public bool hasDurability = false;
    public int maxDurability = 100;
    public ToolType toolType = ToolType.None;


    [Header("Mining Resources")]
    // Tool requirement for resources
    public ToolType requiredTool = ToolType.None;
    public ItemData DropItemData;
    public string InfoText = "Mine";

    // Later add extra data blocks for gear stats / potion effects / etc.
}

public enum ItemCategory
{
    Resource,
    Material,
    Tool,
    Gear,
    Consumable,   // potions, food
    Document,     // papers, scrolls, notes
    Quest,
    Misc
}

public enum ToolType
{
    None,
    Axe,
    Pickaxe,
    Hammer
}


