public interface IInteractable
{
    string GetInfoText(PlayerBootstrap player);// ex: "Chop!", "Pick Up"
    void Interact(PlayerBootstrap player);
    bool CanInteract(PlayerBootstrap player);
}

