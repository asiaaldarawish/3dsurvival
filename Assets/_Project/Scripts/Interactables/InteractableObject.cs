using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    
    [SerializeField] private ItemData itemData;
    [SerializeField] private int amount = 1;

    private bool alreadyTaken = false;

    public void Interact(PlayerBootstrap player)
    {
        if (alreadyTaken) return;
        alreadyTaken = true;

        InventoryEvents.OnItemCollected?.Invoke(itemData, amount);

        Destroy(gameObject);
    }

    public string GetInfoText(PlayerBootstrap player) => "Pick Up";

    public bool CanInteract(PlayerBootstrap p) => !alreadyTaken;

    public void Configure(ItemData data, int count)
    {
        itemData = data;
        amount = count;
        alreadyTaken = false;
    }

}
