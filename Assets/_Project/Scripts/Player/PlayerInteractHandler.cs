using UnityEngine;

public class PlayerInteractHandler : MonoBehaviour
{
    private IInteractable current;
    [SerializeField] private PlayerBootstrap player;
    private readonly System.Collections.Generic.List<IInteractable> interactablesInRange = new();

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

        interactablesInRange.Clear();
        current = null;
        InteractionUIEvents.HideInteractionText?.Invoke();
    }

    private void Update()
    {
        if (!IsUnityNull(current))
            return;

        current = null;
        CleanupInteractables();
        InteractionUIEvents.HideInteractionText?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        if (!interactablesInRange.Contains(interactable))
            interactablesInRange.Add(interactable);

        UpdateCurrentInteractable();
    }

    private void OnTriggerExit(Collider other)
    {
        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        interactablesInRange.Remove(interactable);
        if (current == interactable)
            current = null;

        UpdateCurrentInteractable();
    }

    public void OnInteract()
    {
        if (current != null && current.CanInteract(player))
        {
            current.Interact(player);
            UpdateCurrentInteractable();
        }
    }

    public void OnAttack()
    {
        if (current is IAttackable attackable && attackable.CanAttack(player))
        {
            attackable.Attack(player);
            RefreshInteractionText();
        }
    }

    private void HandleHotbarChanged(int index)
    {
        RefreshInteractionText();
    }

    private void UpdateCurrentInteractable()
    {
        CleanupInteractables();

        IInteractable closest = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (var interactable in interactablesInRange)
        {
            if (IsUnityNull(interactable))
                continue;

            if (interactable is not MonoBehaviour behaviour)
                continue;

            float distanceSqr = (behaviour.transform.position - player.transform.position).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closest = interactable;
            }
        }

        current = closest;
        RefreshInteractionText();
    }

    private void RefreshInteractionText()
    {
        if (player == null)
        {
            InteractionUIEvents.HideInteractionText?.Invoke();
            return;
        }

        if (current == null)
        {
            InteractionUIEvents.HideInteractionText?.Invoke();
            return;
        }

        if (IsUnityNull(current))
        {
            current = null;
            InteractionUIEvents.HideInteractionText?.Invoke();
            return;
        }

        InteractionUIEvents.ShowInteractionText?.Invoke(current.GetInfoText(player));
    }

    private void CleanupInteractables()
    {
        interactablesInRange.RemoveAll(IsUnityNull);
    }

    private static bool IsUnityNull(IInteractable interactable)
    {
        return interactable is Object unityObj && unityObj == null;
    }
}