using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class InventorySlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Refs")]
    public int SlotIndex;
    private InventoryManager inventory;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Slider durabilitySlider;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rect;

    private GameObject dragIcon;
    private RectTransform dragIconRect;

    public void Bind(InventoryManager inv)
    {
        inventory = inv;
    }

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
            if (countText != null)
                countText.gameObject.SetActive(false);
            if (durabilitySlider != null)
                durabilitySlider.gameObject.SetActive(false);
        }
    }

    // TOOLTIP
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

    // CLICK (Split Stack)
    public void OnPointerClick(PointerEventData eventData)
    {
        var slot = inventory.slots[SlotIndex];
        if (slot.IsEmpty) return;

        // Left click + Shift = split stack
        if (InputState.Shift && slot.item.data.stackable)
        {
            inventory.SplitStack(SlotIndex);
        }
    }

    // DRAG & DROP
    public void OnBeginDrag(PointerEventData eventData)
    {
        var slot = inventory.slots[SlotIndex];
        if (slot.IsEmpty) return;

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

public static class InventoryUIEvents
{
    public static Action<int, int> OnSlotSwap;
}


