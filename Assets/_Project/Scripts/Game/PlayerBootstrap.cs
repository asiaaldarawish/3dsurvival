using UnityEngine;

[RequireComponent(typeof(InputReader))]
public class PlayerBootstrap : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerInteractHandler interactHandler;
    [SerializeField] private InputReader inputReader;

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
    }

    private void OnDisable()
    {
        inputReader.OnMove -= movement.Move;
        inputReader.OnJump -= movement.Jump;
        inputReader.OnSprint -= movement.SetSprinting;


        inputReader.OnInteract -= interactHandler.OnInteract;
    }

}

    

