using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingManagerUI : MonoBehaviour
{
    [SerializeField] private CraftingManager craftingManager;
    [SerializeField] private CraftingRecipeUIEntry[] recipeEntries;

    private void OnEnable()
    {
        InventoryManager.OnInventoryChanged += RefreshAll;
        RefreshAll();
    }

    private void OnDisable()
    {
        InventoryManager.OnInventoryChanged -= RefreshAll;
    }

    public void RefreshAll()
    {
        if (craftingManager == null) return;

        foreach (var entry in recipeEntries)
        {
            RefreshEntry(entry);
        }
    }

    private void RefreshEntry(CraftingRecipeUIEntry entry)
    {
        var recipe = entry?.recipe;

        if (entry != null && entry.titleText != null)
        {
            entry.titleText.text = recipe?.OutputItem != null
                ? $"Craft: {recipe.OutputItem.displayName}"
                : "Craft: --";
        }

        if (entry != null && entry.iconImage != null)
        {
            entry.iconImage.sprite = recipe?.OutputItem != null ? recipe.OutputItem.icon : null;
            entry.iconImage.enabled = entry.iconImage.sprite != null;
        }


        if (entry != null && entry.ingredientsText != null)
        {
            entry.ingredientsText.text = BuildIngredientLabel(recipe);
        }

        if (entry != null && entry.craftButton != null)
        {
            entry.craftButton.onClick.RemoveAllListeners();
            if (recipe != null)
                entry.craftButton.onClick.AddListener(() => AttemptCraft(recipe));

            entry.craftButton.interactable = recipe != null && craftingManager.CanCraft(recipe);
        }
    }

    private string BuildIngredientLabel(CraftingRecipe recipe)
    {
        if (recipe == null || recipe.Ingredients == null)
            return string.Empty;

        var sb = new StringBuilder();
        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            var ingredient = recipe.Ingredients[i];
            if (ingredient == null || ingredient.item == null) continue;

            if (sb.Length > 0)
                sb.Append("\n");

            sb.Append($"- {ingredient.amount}x {ingredient.item.displayName}");
        }

        return sb.ToString();
    }

    private void AttemptCraft(CraftingRecipe recipe)
    {
        if (craftingManager.Craft(recipe))
        {
            RefreshAll();
        }
    }
}

[System.Serializable]
public class CraftingRecipeUIEntry
{
    public CraftingRecipe recipe;
    public Button craftButton;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI ingredientsText;
    public Image iconImage;
}