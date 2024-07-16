using UnityEngine;

public class JumpController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;
    private Rigidbody2D rigidBody2D;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] float rayLength = 0.5f;
    private GroundChecker groundChecker;
    private Movable movableComponent;

    private void Awake()
    {
        movableComponent = GetComponent<Movable>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        groundChecker = new GroundChecker(transform, GetComponent<Collider2D>(), groundLayer, rayLength);
    }

    public void TryJump()
    {
        if (groundChecker.IsGrounded())
        {
            Vector2 jumpVelocity = new Vector2(0, jumpForce);
            movableComponent.MoveInDirection(jumpVelocity);
        }
    }
}