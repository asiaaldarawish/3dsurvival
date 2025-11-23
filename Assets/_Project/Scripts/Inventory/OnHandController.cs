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

    private void UpdateHandItemByIndex(int index)
    {
        UpdateHandItem(index);
    }

    private void UpdateHandFromCurrentHotbar()
    {
        UpdateHandItem(hotbar.currentIndex);
    }

    private void UpdateHandItem(int index)
    {
        if (currentHeld != null)
            Destroy(currentHeld);

        var slot = inventory.slots[index];
        if (slot.IsEmpty)
            return;

        var data = slot.item.data;

        GameObject prefab = data.handPrefab;

        if (prefab != null)
        {
            currentHeld = Instantiate(prefab, handPoint);
            currentHeld.transform.localPosition = Vector3.zero;
            currentHeld.transform.localRotation = Quaternion.identity;
        }
    }

}
