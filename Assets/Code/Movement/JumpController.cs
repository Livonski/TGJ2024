using UnityEngine;

public class JumpController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] float rayLength = 0.2f;
    private GroundChecker groundChecker;
    private Movable movable;

    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float jumpForce = 1.0f;
    [SerializeField] private float jumpDuration = 1.0f;

    [SerializeField] private int maxJumpCharges;
    private int jumpCharges;
    private bool jumped;
    private bool eligibleToJump;
    private bool grounded;
    private float jumpTimeElapsed;

    private void Awake()
    {
        movable = GetComponent<Movable>();
        groundChecker = new GroundChecker(transform, GetComponent<Collider2D>(), groundLayer, rayLength);
        jumpCharges = maxJumpCharges;
    }

    private void FixedUpdate()
    {
        grounded = groundChecker.IsGrounded();
        jumpCharges = grounded ? maxJumpCharges : jumpCharges;
        eligibleToJump = grounded ^ (!grounded && jumpCharges > 0 && jumpCharges < maxJumpCharges);
        Vector2 jumpVelocity = calculateJumpVector();
        movable.addVelocity(jumpVelocity);
    }

    public void TryJump()
    {
        if (eligibleToJump)
        {
            jumped = true;
            jumpCharges--;
        }
    }

    public bool hasJumped()
    {
        return jumped;
    }
    private Vector2 calculateJumpVector()
    {
        if (!jumped)
            return Vector2.zero;
        float verticalVelocity = jumpForce + jumpCurve.Evaluate(jumpTimeElapsed / jumpDuration) * jumpDuration;
        //Debug.Log(verticalVelocity);
        jumpTimeElapsed += Time.fixedDeltaTime;
        if (jumpTimeElapsed >= jumpDuration)
        {
            jumped = false;
            jumpTimeElapsed = 0;
        }
        return new Vector2(0, verticalVelocity);
    }
}