using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [Header("UI")]
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private CraftingManagerUI craftingUI;
    [SerializeField] private InventoryManager inventoryManager;

    private PlayerInteractHandler playerHandler;
    private bool isOpen;

    private void Awake()
    {
        if (craftingUI == null && craftingPanel != null)
            craftingUI = craftingPanel.GetComponentInChildren<CraftingManagerUI>(true);

        if (craftingPanel == null && craftingUI != null)
            craftingPanel = craftingUI.gameObject;

        if (craftingUI == null)
            craftingUI = FindFirstObjectByType<CraftingManagerUI>(FindObjectsInactive.Include);

        if (craftingPanel == null)
            craftingPanel = craftingUI != null ? craftingUI.gameObject : null;


        ClosePanel();
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

        if (isOpen)
            ClosePanel();

        InteractionUIEvents.HideInteractionText?.Invoke();
    }

    public string GetInfoText(PlayerBootstrap player) => "Craft [E]";

    public void Interact(PlayerBootstrap player)
    {
        if (isOpen)
            ClosePanel();
        else
            OpenPanel(player);
    }

    public bool CanInteract(PlayerBootstrap player) => true;

    private void OpenPanel(PlayerBootstrap player)
    {
        inventoryManager = FindInventoryManager();
        isOpen = true;

        if (craftingPanel != null)
            craftingPanel.SetActive(true);

        craftingUI?.RefreshAll();
        inventoryManager?.OpenInventoryPanel();
    }

    private void ClosePanel()
    {
        isOpen = false;
        if (craftingPanel != null)
            craftingPanel.SetActive(false);

        inventoryManager?.CloseInventoryPanel();
    }

    private InventoryManager FindInventoryManager()
    {
        if (inventoryManager != null)
            return inventoryManager;

        return FindFirstObjectByType<InventoryManager>(FindObjectsInactive.Include);
    }

    
}