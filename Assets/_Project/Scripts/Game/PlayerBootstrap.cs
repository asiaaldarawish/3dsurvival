using UnityEngine;

[RequireComponent(typeof(InputReader))]
public class PlayerBootstrap : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerInteractHandler interactHandler;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private HotbarUI hotbar;

    private void Reset()
    {
        inputReader = GetComponent<InputReader>();
    }

    private void OnEnable()
    {
        inputReader.OnMove += movement.Move;
        inputReader.OnJump += movement.Jump;
        inputReader.OnSprint += movement.SetSprinting;

        inputReader.OnInteract += interactHandler.OnInteract;
        inputReader.OnAttack += interactHandler.OnAttack;
    }

    private void OnDisable()
    {
        inputReader.OnMove -= movement.Move;
        inputReader.OnJump -= movement.Jump;
        inputReader.OnSprint -= movement.SetSprinting;


        inputReader.OnInteract -= interactHandler.OnInteract;
        inputReader.OnAttack -= interactHandler.OnAttack;
    }

    public InventoryItem GetCurrentHotbarItem()
    {
        if (inventory == null || hotbar == null)
            return null;

        if (hotbar.currentIndex < 0 || hotbar.currentIndex >= inventory.slots.Length)
            return null;

        return inventory.slots[hotbar.currentIndex].item;
    }
}



