using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/Material")]
public class MaterialItemData : ItemData
{
    private void OnValidate()
    {
        category = ItemCategory.Material;
        stackable = true;
        maxStack = Mathf.Max(1, maxStack);
    }
}
