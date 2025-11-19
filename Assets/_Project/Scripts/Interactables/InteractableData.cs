using UnityEngine;

public abstract class InteractableData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string infoText;     // ex: “Chop!”, “Pick Up”
}
