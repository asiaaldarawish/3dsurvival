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

        if (recipeEntries == null) return;

        for (int i = 0; i < recipeEntries.Length; i++)
        {
            RefreshEntry(recipeEntries[i]);
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
        if (recipe == null)
            return string.Empty;

        var requirements = recipe.Requirements;
        var sb = new StringBuilder();

        AppendRequirement(sb, requirements.requirement1);
        AppendRequirement(sb, requirements.requirement2);
        AppendRequirement(sb, requirements.requirement3);
        AppendRequirement(sb, requirements.requirement4);

        return sb.ToString().TrimStart('\n');
    }

    private void AppendRequirement(StringBuilder sb, CraftingRequirement requirement)
    {
        if (requirement.item == null || requirement.amount <= 0)
            return;

        sb.Append('\n');
        sb.Append($"- {requirement.amount}x {requirement.item.displayName}");
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