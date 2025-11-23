using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Slider durabilitySlider;

    public void Clear()
    {
        SetItem(null);
    }

    public void SetItem(InventoryItem item)
    {
        if (item == null || item.data == null)
        {
            if (icon != null) icon.enabled = false;
            if (countText != null) countText.gameObject.SetActive(false);
            if (durabilitySlider != null) durabilitySlider.gameObject.SetActive(false);
            return;
        }

        var data = item.data;

        if (icon != null)
        {
            icon.sprite = data.icon;
            icon.enabled = true;
        }

        // Stackable item (materials, resources, maybe potions)
        if (data.stackable)
        {
            if (countText != null)
            {
                countText.gameObject.SetActive(true);
                countText.text = item.count.ToString();
            }

            if (durabilitySlider != null)
                durabilitySlider.gameObject.SetActive(false);
        }
        // Non-stackable with durability (tools, gear)
        else if (data.hasDurability)
        {
            if (countText != null)
                countText.gameObject.SetActive(false);

            if (durabilitySlider != null)
            {
                durabilitySlider.gameObject.SetActive(true);
                durabilitySlider.maxValue = data.maxDurability;
                durabilitySlider.value = item.durability;
            }
        }
        else
        {
            // Non-stackable, no durability (e.g. quest item, document, potion)
            if (countText != null)
                countText.gameObject.SetActive(false);
            if (durabilitySlider != null)
                durabilitySlider.gameObject.SetActive(false);
        }
    }
}
