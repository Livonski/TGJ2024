using UnityEngine;

public class JumpController : MonoBehaviour
{
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
        groundChecker = GetComponent<GroundChecker>();
        jumpCharges = maxJumpCharges;
    }

    private void FixedUpdate()
    {
        eligibleToJump = grounded || (!grounded && jumpCharges > 0);

        grounded = groundChecker.IsGrounded();
        if (grounded)
        {
            jumpCharges = maxJumpCharges;
        }
        if (jumped)
        {
            Vector2 jumpVelocity = calculateJumpVector();
            movable.addVelocity(jumpVelocity);
        }
    }

    public bool TryJump()
    {
        if (eligibleToJump)
        {
            jumped = true;
            jumpCharges--;
            return true;
        }
        return false;
    }

    public bool hasJumped()
    {
        return jumped;
    }

    public void setMaxJumpCharges(int newMAxJumpCharges)
    {
        maxJumpCharges = newMAxJumpCharges;
    }

    private Vector2 calculateJumpVector()
    {
        if (!jumped)
            return Vector2.zero;
        float verticalVelocity = jumpForce + jumpCurve.Evaluate(jumpTimeElapsed / jumpDuration) * jumpDuration;
        jumpTimeElapsed += Time.fixedDeltaTime;
        if (jumpTimeElapsed >= jumpDuration)
        {
            jumped = false;
            jumpTimeElapsed = 0;
        }
        return new Vector2(0, verticalVelocity);
    }
}