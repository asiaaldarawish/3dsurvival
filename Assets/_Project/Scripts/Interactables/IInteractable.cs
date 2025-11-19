public interface IInteractable
{
    string GetInfoText();          // ex: "Chop!", "Pick Up"
    InteractableType GetInteractableType();
    void Interact(PlayerBootstrap player);
    bool CanInteract(PlayerBootstrap player);
}

public enum InteractableType
{
    Resource,
    Material,
    Tool
}
