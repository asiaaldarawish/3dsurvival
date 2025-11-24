using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    public static ItemTooltipUI Instance;

    [SerializeField] private CanvasGroup cg;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI extraText;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(ItemData data, InventoryItem item)
    {
        icon.sprite = data.icon;

        nameText.text = data.displayName;
        //nameText.color = data.rarityColor;

        descriptionText.text = data.description;

        // Show count or durability
        if (data.stackable)
            extraText.text = $"Count: {item.count}";
        else if (data.hasDurability)
            extraText.text = $"Durability: {item.durability}/{data.maxDurability}";
        else
            extraText.text = "";

        cg.alpha = 1;
        cg.blocksRaycasts = false;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        cg.alpha = 0;
        gameObject.SetActive(false);
    }

    //private void Update()
    //{
    //    // follow mouse
    //    transform.position = Input.mousePosition + new Vector3(15, -15, 0);
    //}
}
