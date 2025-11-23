public interface IInteractable
{
    string GetInfoText();          // ex: "Chop!", "Pick Up"
    void Interact(PlayerBootstrap player);
    bool CanInteract(PlayerBootstrap player);
}

