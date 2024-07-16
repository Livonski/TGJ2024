using UnityEngine;

public class Movable : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float accelerationTime = 1.0f;     // Time it takes to reach max speed
    [SerializeField] private float decelerationTime = 1.0f;     // Time it takes to stop from max speed
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] float rayLength = 0.5f;
    [SerializeField] private AnimationCurve jumpCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

    [SerializeField] private float jumpForce = 1.0f;
    [SerializeField] private float jumpDuration = 1.0f;
    [SerializeField] private float gravity = 1.0f;


    private Rigidbody2D rigidBody2D;
    private GroundChecker groundChecker;
    private SurfaceSlider surfaceSlider;

    private Vector2 currentVelocity;
    private Vector2 movementPart;
    private Vector2 jumpPart;

    private float accelerationPerSecond;
    private float decelerationPerSecond;

    private bool isJumping;
    private float jumpTimeElapsed;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();

        groundChecker = new GroundChecker(transform, GetComponent<Collider2D>(), groundLayer, rayLength);
        surfaceSlider = new SurfaceSlider(maxSpeed);

        accelerationPerSecond = maxSpeed / accelerationTime;
        decelerationPerSecond = maxSpeed / decelerationTime;
    }

    private void FixedUpdate()
    {
        applyVelocity();
    }

    private void applyVelocity()
    {
        // jumpPart += isJumping ? calculateJumpVector() : (groundChecker.IsGrounded() ? Vector2.zero : new Vector2(0, -gravity));
        if (groundChecker.IsGrounded() & !isJumping)
        {
            jumpPart = new Vector2(0, -1f);
        }
        else
        {
            jumpPart += isJumping ? calculateJumpVector() : new Vector2(0, -gravity);
        }
        // jumpPart.y = Mathf.Clamp(jumpPart.y,-gravity, float.MaxValue);
        // gravityPart = isJumping ? Vector2.zero : new Vector2(0, -gravity);

        Debug.Log("jump part : " + jumpPart);
        currentVelocity = movementPart + jumpPart;
        rigidBody2D.MovePosition(rigidBody2D.position + currentVelocity * Time.fixedDeltaTime);
    }

    public void MoveInDirection(Vector2 inputDirection)
    {
        Vector2 groundNormal = groundChecker.GetGroundNormal();
        if (!groundChecker.IsGrounded())
        {
            return;
        }
        Vector2 targetVelocity = surfaceSlider.projectVector(groundNormal, inputDirection);
        Vector2 velocityDifference = targetVelocity - movementPart;
        float accelerationRate = Mathf.Abs(velocityDifference.x) > 0.01f ? accelerationPerSecond : decelerationPerSecond;
        Vector2 movementIncrement = velocityDifference.normalized * accelerationRate * Time.fixedDeltaTime;

        // Clamp the movement increment to not exceed the velocity difference
        movementIncrement = movementIncrement.magnitude > velocityDifference.magnitude ? velocityDifference : movementIncrement;
        movementPart += movementIncrement;
        // Debug.Log("movement part" + movementPart);
    }

    private Vector2 calculateJumpVector()
    {
        float verticalVelocity = jumpForce + jumpCurve.Evaluate(jumpTimeElapsed / jumpDuration) * jumpDuration;
        Debug.Log(verticalVelocity);
        jumpTimeElapsed += Time.fixedDeltaTime;
        if (jumpTimeElapsed >= jumpDuration)
        {
            isJumping = false;
        }
        return new Vector2(0, verticalVelocity);
    }

    public void Jump()
    {
        if (groundChecker.IsGrounded())
        {
            isJumping = true;
            jumpTimeElapsed = 0;
        }
    }

}