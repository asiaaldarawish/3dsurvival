using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Output")]
    public ItemData outputItem;
    public int outputAmount = 1;

    [Header("Ingredients (max 4)")]
    public CraftingRequirements requirements;

    public ItemData OutputItem => outputItem;
    public int OutputAmount => Mathf.Max(1, outputAmount);
    public CraftingRequirements Requirements => requirements;
}

[Serializable]
public struct CraftingRequirement
{
    public ItemData item;
    public int amount;
}

[Serializable]
public struct CraftingRequirements
{
    public CraftingRequirement requirement1;
    public CraftingRequirement requirement2;
    public CraftingRequirement requirement3;
    public CraftingRequirement requirement4;
}