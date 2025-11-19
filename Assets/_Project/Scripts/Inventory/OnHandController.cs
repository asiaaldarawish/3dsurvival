using UnityEngine;

public class OnHandController : MonoBehaviour
{
    [SerializeField] private Transform handPoint;
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private HotbarUI hotbar;

    private GameObject currentHeld;

    private void OnEnable()
    {
        HotbarUI.OnHotbarChanged += UpdateHandItemByIndex;
        InventoryManager.OnInventoryChanged += UpdateHandFromCurrentHotbar;
    }

    private void OnDisable()
    {
        HotbarUI.OnHotbarChanged -= UpdateHandItemByIndex;
        InventoryManager.OnInventoryChanged -= UpdateHandFromCurrentHotbar;
    }

    private void Start()
    {
        UpdateHandFromCurrentHotbar();
    }

    // ============================
    // HOTBAR EVENT → needs an int
    // ============================
    private void UpdateHandItemByIndex(int index)
    {
        UpdateHandItem(index);
    }

    // ============================================
    // INVENTORY CHANGE → use current hotbar index
    // ============================================
    private void UpdateHandFromCurrentHotbar()
    {
        UpdateHandItem(hotbar.currentIndex);
    }

    // ======================================
    // THE REAL LOGIC (shared by both events)
    // ======================================
    private void UpdateHandItem(int index)
    {
        Destroy(currentHeld);

        var slot = inventory.slots[index];
        if (slot.item == null)
            return;

        var data = slot.item.data;

        GameObject prefab = null;

        if (data is MaterialData mat) prefab = mat.handPrefab;
        else if (data is ToolData tool) prefab = tool.handPrefab;

        if (prefab != null)
        {
            currentHeld = Instantiate(prefab, handPoint);
            currentHeld.transform.localPosition = Vector3.zero;
            currentHeld.transform.localRotation = Quaternion.identity;
        }
    }
}
