using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Movable movableComponent;
    private JumpController jumpController;

    private void Awake()
    {
        movableComponent = GetComponent<Movable>();
        jumpController = GetComponent<JumpController>();
    }

    private void Update()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), 0);
        if (inputVector != Vector2.zero)
            movableComponent.MoveInDirection(inputVector);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            movableComponent.Jump();
        }
    }
}