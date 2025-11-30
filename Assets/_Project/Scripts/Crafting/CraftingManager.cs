using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    public IReadOnlyList<CraftingRecipe> Recipes => recipes;

    public event Action<CraftingRecipe> OnCrafted;

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = GetComponent<InventoryManager>();
    }

    public bool CanCraft(CraftingRecipe recipe)
    {
        if (recipe == null || inventoryManager == null) return false;
        if (recipe.OutputItem == null) return false;

        return inventoryManager.Model.HasItems(recipe.Ingredients);
    }

    public bool Craft(CraftingRecipe recipe)
    {
        if (!CanCraft(recipe))
            return false;

        if (!inventoryManager.Model.ConsumeItems(recipe.Ingredients))
            return false;

        int leftover = inventoryManager.Model.AddItem(recipe.OutputItem, recipe.OutputAmount);
        if (leftover > 0)
        {
            Debug.Log($"Not enough inventory space. Leftover crafted items: {leftover}");
        }

        OnCrafted?.Invoke(recipe);
        return true;
    }
}