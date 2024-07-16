using UnityEngine;

public class Movable : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float accelerationTime = 1.0f;     // Time it takes to reach max speed
    [SerializeField] private float decelerationTime = 1.0f;     // Time it takes to stop from max speed
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] float rayLength = 0.5f;
    [SerializeField] private AnimationCurve jumpCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
    [SerializeField] private float jumpDuration = 1.0f;
    [SerializeField] private float gravitation = 1.0f;


    private Rigidbody2D rigidBody2D;
    private GroundChecker groundChecker;
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
        accelerationPerSecond = maxSpeed / accelerationTime;
        decelerationPerSecond = maxSpeed / decelerationTime;
    }

    private void FixedUpdate()
    {
        applyVelocity();
    }

    private void applyVelocity()
    {
        jumpPart = isJumping ? calculateJumpVector() : new Vector2(0,0);
        currentVelocity = movementPart + jumpPart;
        rigidBody2D.MovePosition(rigidBody2D.position + currentVelocity * Time.fixedDeltaTime);
    }

    public void MoveInDirection(Vector2 inputDirection)
    {
        Vector2 groundNormal = groundChecker.GetGroundNormal();
        Vector2 forward = Vector2.Perpendicular(groundNormal) * Mathf.Sign(Vector2.Dot(groundNormal, Vector2.down));

        //if (!groundChecker.IsGrounded())
        //{
        //    currentVelocity = new Vector2(currentVelocity.x * 0.95f, currentVelocity.y); // Apply air resistance
        //    return;
        //}

        float inputProjected = Vector2.Dot(inputDirection, forward);
        Vector2 targetVelocity = forward * inputProjected * maxSpeed;
        Vector2 velocityDifference = targetVelocity - movementPart;
        float accelerationRate = Mathf.Abs(velocityDifference.x) > 0.01f ? accelerationPerSecond : decelerationPerSecond;
        Vector2 movementIncrement = velocityDifference.normalized * accelerationRate * Time.fixedDeltaTime;

        // Clamp the movement increment to not exceed the velocity difference
        movementIncrement = movementIncrement.magnitude > velocityDifference.magnitude ? velocityDifference : movementIncrement;
        movementPart += movementIncrement;
    }

    private Vector2 calculateJumpVector()
    {
        float verticalVelocity = jumpCurve.Evaluate(jumpTimeElapsed / jumpDuration) * jumpDuration;
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