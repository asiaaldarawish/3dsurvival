using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    private PlayerInteractHandler player;


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

    public string GetInfoText() => "Pick Up";

    public bool CanInteract(PlayerBootstrap p) => !alreadyTaken;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.GetComponent<PlayerInteractHandler>();
        player.SetCurrent(this);

        InteractionUIEvents.ShowInteractionText?.Invoke(GetInfoText());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player.ClearCurrent(this);

        InteractionUIEvents.HideInteractionText?.Invoke();
    }


    private void OnDestroy()
    {
        if (player != null)
            player.ClearCurrent(this);

        InteractionUIEvents.HideInteractionText?.Invoke();
    }
}
