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
    public bool stackable = false;
    public int maxStack = 1;
    protected virtual void OnValidate()
    {
        stackable = false;
        maxStack = 1;
    }
}




