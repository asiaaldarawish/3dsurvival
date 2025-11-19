using UnityEngine;
using UnityEngine.UI;

public class ToolSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Slider durabilitySlider;

    private ToolData toolData;

    // Initialize tool item in UI
    public void Set(ToolData data, int durability)
    {
        toolData = data;

        if (icon != null && data.icon != null)
            icon.sprite = data.icon;

        if (durabilitySlider != null)
        {
            durabilitySlider.maxValue = data.durability;
            durabilitySlider.value = durability;
        }
    }

    // Optional: If you decrease durability
    public void UpdateDurability(int newValue)
    {
        durabilitySlider.value = newValue;
    }
}
