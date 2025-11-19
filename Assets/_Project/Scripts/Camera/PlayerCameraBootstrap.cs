using UnityEngine;

public class PlayerCameraBootstrap : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private PlayerCamera cam;

    private void OnEnable()
    {
        input.OnLook += cam.HandleLook; 
        input.OnZoom += cam.HandleZoom;
    }

    private void OnDisable()
    {
        input.OnLook -= cam.HandleLook;
        input.OnZoom -= cam.HandleZoom;
    }
}
