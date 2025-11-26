using UnityEngine;

public class ResourceNode : MonoBehaviour, IInteractable, IAttackable
{
    [Header("Resource")]
    [SerializeField] private ItemData resourceData;
    [SerializeField] private ItemData requiredTool;
    [SerializeField] private int maxHealth = 3;

    [Header("Drops")]
    private ItemData dropItem;
    [SerializeField] private int dropAmount = 1;
    [SerializeField] private float dropRadius = 0.5f;
    [SerializeField] private Transform dropOrigin;

    private int currentHealth;
    private bool harvested;

    private PlayerInteractHandler playerHandler;

    private void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);

        if (dropItem == null)
            dropItem = resourceData.DropItemData;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerHandler = other.GetComponent<PlayerInteractHandler>();

        if (playerHandler != null)
            playerHandler.SetCurrent(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (playerHandler != null)
            playerHandler.ClearCurrent(this);

        playerHandler = null;

        InteractionUIEvents.HideInteractionText?.Invoke();
    }

    private void OnDestroy()
    {
        if (playerHandler != null)
            playerHandler.ClearCurrent(this);

        InteractionUIEvents.HideInteractionText?.Invoke();
    }

    public string GetInfoText(PlayerBootstrap player)
    {
        if (harvested) return string.Empty;

        if (!HasCorrectTool(player))
            return requiredTool != null ? $"Use {requiredTool.displayName}" : "Requires tool";

        return resourceData.InfoText;
    }

    public void Interact(PlayerBootstrap player)
    {
        if (CanInteract(player))
            Attack(player);
    }

    public bool CanInteract(PlayerBootstrap player)
    {
        return !harvested && HasCorrectTool(player);
    }

    public bool CanAttack(PlayerBootstrap player)
    {
        return CanInteract(player);
    }

    public void Attack(PlayerBootstrap player)
    {
        if (!CanAttack(player))
        {
            InteractionUIEvents.ShowInteractionText?.Invoke(GetInfoText(player));
            return;
        }

        currentHealth--;

        if (currentHealth <= 0)
        {
            Harvest();
        }
        else
        {
            InteractionUIEvents.ShowInteractionText?.Invoke(GetInfoText(player));
        }
    }

    private bool HasCorrectTool(PlayerBootstrap player)
    {
        if (player == null || requiredTool == null)
            return false;

        var held = player.GetCurrentHotbarItem();
        return held != null && held.data == requiredTool;
    }

    private void Harvest()
    {
        if (harvested) return;

        harvested = true;
        SpawnDrops();
        Destroy(gameObject);
    }

    private void SpawnDrops()
    {
        if (dropItem == null) return;

        Vector3 origin = dropOrigin != null ? dropOrigin.position : transform.position;

        for (int i = 0; i < dropAmount; i++)
        {
            Vector3 offset = Random.insideUnitSphere * dropRadius;
            offset.y = 0f;
            Vector3 spawnPos = origin + offset;

            if (dropItem.worldPrefab != null)
            {
                var pickup = Instantiate(dropItem.worldPrefab, spawnPos, Quaternion.identity);
                var interactable = pickup.GetComponent<InteractableObject>();
                if (interactable != null)
                {
                    interactable.Configure(dropItem, 1);
                }
            }
            else
            {
                InventoryEvents.OnItemCollected?.Invoke(dropItem, 1);
            }
        }
    }
}