using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/Material")]
public class MaterialItemData : ItemData
{
    protected override void OnValidate()
    {
        stackable = true;
        maxStack = Mathf.Max(1, maxStack);
    }
}