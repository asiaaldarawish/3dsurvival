using UnityEngine;

public class PlayerInteractHandler : MonoBehaviour
{
    private IInteractable current;
    [SerializeField] private PlayerBootstrap player;

    private void Awake()
    {
        if (player == null)
            player = GetComponent<PlayerBootstrap>();

        if (player == null)
            player = GetComponentInParent<PlayerBootstrap>();
    }

    private void OnEnable()
    {
        HotbarUI.OnHotbarChanged += HandleHotbarChanged;
    }

    private void OnDisable()
    {
        HotbarUI.OnHotbarChanged -= HandleHotbarChanged;
    }

    public void SetCurrent(IInteractable interactable)
    {
        current = interactable;

        RefreshInteractionText();
    }

    public void ClearCurrent(IInteractable interactable)
    {
        if (current == interactable)
            current = null;
    }

    public void OnInteract()
    {
        if (current != null && current.CanInteract(player))
        {
            current.Interact(player);
        }
    }

    public void OnAttack()
    {
        if (current is IAttackable attackable && attackable.CanAttack(player))
        {
            attackable.Attack(player);
        }
    }

    private void HandleHotbarChanged(int index)
    {
        RefreshInteractionText();
    }

    private void RefreshInteractionText()
    {
        if (current == null || player == null) return;

        InteractionUIEvents.ShowInteractionText?.Invoke(current.GetInfoText(player));
    }
}
