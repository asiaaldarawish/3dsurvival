using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public static class InventoryUIEvents
{
    public static Action<int, int> OnSlotSwap;

}


public class InventorySlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Setup")]
    public int SlotIndex;
    public InventoryManager inventory;

    // internal UI refs
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rect;

    // drag visuals
    private GameObject dragIcon;
    private RectTransform dragIconRect;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }

    public void Clear()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    public void SetMaterial(InventoryItem item)
    {
        var obj = Instantiate(inventory.materialSlotPrefab, transform);
        obj.GetComponent<MaterialSlotUI>().Set(item.data as MaterialData, item.count);
    }

    public void SetTool(InventoryItem item)
    {
        var obj = Instantiate(inventory.toolSlotPrefab, transform);
        obj.GetComponent<ToolSlotUI>().Set(item.data as ToolData, item.durability);
    }

    //==============================================================
    // DRAG START
    //==============================================================
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventory.slots[SlotIndex].item == null)
            return; // don't drag empty slot

        // Create a drag icon
        dragIcon = new GameObject("DragIcon", typeof(CanvasGroup), typeof(Image));
        dragIcon.transform.SetParent(canvas.transform, false);

        var img = dragIcon.GetComponent<Image>();
        img.raycastTarget = false; // makes icon ignore input

        // Take icon sprite from slot
        var childImg = GetComponentInChildren<Image>();
        img.sprite = childImg ? childImg.sprite : null;

        dragIconRect = dragIcon.GetComponent<RectTransform>();
        dragIconRect.sizeDelta = rect.sizeDelta;

        canvasGroup.alpha = 0.4f;
        canvasGroup.blocksRaycasts = false;
    }

    //==============================================================
    // DRAGGING
    //==============================================================
    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null) return;

        dragIconRect.position = eventData.position;
    }

    //==============================================================
    // DRAG END
    //==============================================================
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (dragIcon != null)
            Destroy(dragIcon);
    }

    //==============================================================
    // WHEN DROPPED ON ANOTHER SLOT
    //==============================================================
    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag.GetComponent<InventorySlotUI>();
        if (dragged == null) return;

        if (dragged.SlotIndex == SlotIndex) return;

        InventoryUIEvents.OnSlotSwap?.Invoke(dragged.SlotIndex, SlotIndex);
    }
}
