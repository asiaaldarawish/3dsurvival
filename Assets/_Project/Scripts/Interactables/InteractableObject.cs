using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractableData data;

    private bool inRange = false;
    private PlayerInteractHandler player;

    public string GetInfoText() => data.infoText;

    public bool CanInteract(PlayerBootstrap p) => inRange;

    public InteractableType GetInteractableType()
    {
        if (data is MaterialData) return InteractableType.Material;
        if (data is ToolData) return InteractableType.Tool;
        return InteractableType.Resource;
    }

    private bool alreadyInteracted = false;

    public void Interact(PlayerBootstrap p)
    {
        if (alreadyInteracted) return;
        alreadyInteracted = true;


        // send inventory event
        switch (GetInteractableType())
        {
            case InteractableType.Material:
                InventoryEvents.OnMaterialCollected?.Invoke((MaterialData)data);
                break;

            case InteractableType.Tool:
                InventoryEvents.OnToolCollected?.Invoke((ToolData)data);
                break;
        }

        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        inRange = true;
        player = other.GetComponent<PlayerInteractHandler>();
        player.SetCurrent(this);

        InteractionUIEvents.ShowInteractionText?.Invoke(GetInfoText());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        inRange = false;
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
