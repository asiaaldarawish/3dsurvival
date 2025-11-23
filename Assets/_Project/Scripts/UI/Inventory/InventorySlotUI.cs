using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using TMPro;

public static class InventoryUIEvents
{
    public static Action<int, int> OnSlotSwap;
}


public class InventorySlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Refs")]
    public int SlotIndex;
    public InventoryManager inventory;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Slider durabilitySlider;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rect;

    // drag ghost
    private GameObject dragIcon;
    private RectTransform dragIconRect;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();

        if (icon != null) icon.enabled = false;
        if (countText != null) countText.gameObject.SetActive(false);
        if (durabilitySlider != null) durabilitySlider.gameObject.SetActive(false);
    }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        var slot = inventory.slots[SlotIndex];
        if (slot.IsEmpty) return;

        ItemTooltipUI.Instance.Show(slot.item.data, slot.item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipUI.Instance.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var slot = inventory.slots[SlotIndex];
        if (slot.IsEmpty) return;

        // SHIFT + leftclick Click = split stack
        if (InputState.Shift && slot.item.data.stackable)
        {
            inventory.SplitStack(SlotIndex);
        }

    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        var slot = inventory.slots[SlotIndex];
        if (slot.IsEmpty)
            return;

        // ghost icon
        dragIcon = new GameObject("DragIcon", typeof(CanvasGroup), typeof(Image));
        dragIcon.transform.SetParent(canvas.transform, false);

        var img = dragIcon.GetComponent<Image>();
        img.raycastTarget = false;

        if (icon != null)
            img.sprite = icon.sprite;

        dragIconRect = dragIcon.GetComponent<RectTransform>();
        dragIconRect.sizeDelta = rect.sizeDelta;

        canvasGroup.alpha = 0.4f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconRect == null) return;
        dragIconRect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (dragIcon != null)
            Destroy(dragIcon);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (dragged == null) return;
        if (dragged.SlotIndex == SlotIndex) return;

        InventoryUIEvents.OnSlotSwap?.Invoke(dragged.SlotIndex, SlotIndex);
    }
}
