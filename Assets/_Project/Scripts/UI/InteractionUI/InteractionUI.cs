using UnityEngine;
using TMPro;

using System;

public static class InteractionUIEvents
{
    public static Action<string> ShowInteractionText;
    public static Action HideInteractionText;
}


public class InteractionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    void OnEnable()
    {
        InteractionUIEvents.ShowInteractionText += Show;
        InteractionUIEvents.HideInteractionText += Hide;
    }

    void OnDisable()
    {
        InteractionUIEvents.ShowInteractionText -= Show;
        InteractionUIEvents.HideInteractionText -= Hide;
    }

    private void Show(string info)
    {
        text.text = info;
        text.gameObject.SetActive(true);
    }

    private void Hide()
    {
        text.gameObject.SetActive(false);
    }
}
