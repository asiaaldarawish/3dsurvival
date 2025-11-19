using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UIElements;

public class PlayerCamera : MonoBehaviour
{
    [Header("zoom References")]
    [SerializeField] private CinemachineCamera cinemachineCam;
    [SerializeField] private float zoomSpeed = 4f;
    [SerializeField] private float minDistance = 1.5f;
    [SerializeField] private float maxDistance = 5f;

    private CinemachineFollow follow;


    [Header("Look References")]
    [SerializeField] private Transform pivot; // the pivot the camera follows
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 60f;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        if (cinemachineCam == null)
            cinemachineCam = FindFirstObjectByType<CinemachineCamera>();

        follow = cinemachineCam.GetComponent<CinemachineFollow>();
        if (follow == null)
            Debug.LogError("CinemachineFollow component not found on CinemachineCamera!");
    }

    public void HandleZoom(float scrollValue)
    {
        if (follow == null) return;

        Vector3 offset = follow.FollowOffset;
        offset.z -= scrollValue * zoomSpeed * Time.deltaTime;
        offset.z = Mathf.Clamp(offset.z, -maxDistance, -minDistance);
        follow.FollowOffset = offset;
    }

    public void HandleLook(Vector2 lookInput)
    {
        yaw += lookInput.x * sensitivity * Time.deltaTime;
        pitch -= lookInput.y * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        pivot.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
