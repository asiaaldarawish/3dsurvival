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

    public string GetInfoText(PlayerBootstrap player) => "Pick Up";

    public bool CanInteract(PlayerBootstrap p) => !alreadyTaken;

    public void Configure(ItemData data, int count)
    {
        itemData = data;
        amount = count;
        alreadyTaken = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.GetComponent<PlayerInteractHandler>();

        if (player == null)
            player = other.GetComponentInParent<PlayerInteractHandler>();

        if (player != null)
            player.SetCurrent(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (player != null)
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
