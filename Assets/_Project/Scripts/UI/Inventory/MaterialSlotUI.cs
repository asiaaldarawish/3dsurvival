using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MaterialSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;

    private MaterialData materialData;

    public void Set(MaterialData data, int count)
    {
        materialData = data;

        if (icon != null && data.icon != null)
            icon.sprite = data.icon;

        if (countText != null)
            countText.text = count.ToString();
    }

    public void UpdateCount(int count)
    {
        if (countText != null)
            countText.text = count.ToString();
    }
}
