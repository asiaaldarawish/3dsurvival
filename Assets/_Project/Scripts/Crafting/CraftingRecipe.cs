using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Output")]
    public ItemData outputItem;
    public int outputAmount = 1;

    [Header("Ingredients")]
    public CraftingIngredient[] ingredients;

    public ItemData OutputItem => outputItem;
    public int OutputAmount => Mathf.Max(1, outputAmount);
    public IReadOnlyList<CraftingIngredient> Ingredients => ingredients;
}

[Serializable]
public class CraftingIngredient
{
    public ItemData item;
    public int amount = 1;
}