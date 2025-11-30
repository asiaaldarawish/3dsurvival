using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/Resource")]
public class ResourceItemData : ItemData
{
    [Header("Resource Requirements")]
    public ToolType requiredTool = ToolType.None;

    [Header("Resource Drop")]
    public MaterialItemData dropItemData;
    public string infoText = "Mine";
}
