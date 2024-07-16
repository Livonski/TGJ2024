using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Movable movableComponent;

    private void Awake()
    {
        movableComponent = GetComponent<Movable>();
    }

    private void Update()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), 0);
        //if (inputVector != Vector2.zero)
            movableComponent.MoveInDirection(inputVector);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            movableComponent.Jump();
        }
    }
}