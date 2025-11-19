using UnityEngine;

public class PlayerInteractHandler : MonoBehaviour
{
    private IInteractable current;

    public void SetCurrent(IInteractable interactable)
    {
        current = interactable;
    }

    public void ClearCurrent(IInteractable interactable)
    {
        if (current == interactable)
            current = null;
    }

    public void OnInteract()
    {
        if (current != null && current.CanInteract(null))
        {
            current.Interact(null); // you can pass player later
        }
    }
}
