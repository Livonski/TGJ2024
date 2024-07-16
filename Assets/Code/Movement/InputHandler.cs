using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Movable movable;
    private JumpController jumpController;

    private void Awake()
    {
        movable = GetComponent<Movable>();
        jumpController = GetComponent<JumpController>();
    }

    private void Update()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), 0);
        //if (inputVector != Vector2.zero)
        movable.MoveInDirection(inputVector);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpController.TryJump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            movable.Dash();
        }
    }
}