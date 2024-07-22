using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Movable : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float accelerationTime = 1.0f;     // Time it takes to reach max speed
    [SerializeField] private float decelerationTime = 1.0f;     // Time it takes to stop from max speed
    private float accelerationPerSecond;
    private float decelerationPerSecond;

    private Rigidbody2D rigidBody2D;
    private GroundChecker groundChecker;

    [SerializeField] private float gravity = 1.0f;
    [SerializeField] private Vector2 gravityVelocity;

    private SurfaceSlider surfaceSlider;
    private JumpController jumpController;
    private DashController dashController;

    private bool hasDashed = false;
    private bool hasJumped = false;

    private Vector2 currentVelocity;
    private List<Vector2> velocities;
    private Vector2 movementPart;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        jumpController = GetComponent<JumpController>();
        dashController = GetComponent<DashController>();
        groundChecker = GetComponent<GroundChecker>();

        surfaceSlider = new SurfaceSlider(maxSpeed);

        accelerationPerSecond = maxSpeed / accelerationTime;
        decelerationPerSecond = maxSpeed / decelerationTime;

        velocities = new List<Vector2>();
    }

    private void FixedUpdate()
    {
        calculateGravity();
        applyVelocity();
    }
    private void applyVelocity()
    {
        if (jumpController != null)
            hasJumped = jumpController.hasJumped();
        if (dashController != null)
            hasDashed = dashController.hasDashed();

        foreach (Vector2 v in velocities)
        {
            currentVelocity += v;
        }
        currentVelocity += (!hasDashed && !hasJumped) ? gravityVelocity : Vector2.zero;
        currentVelocity += movementPart;
        rigidBody2D.MovePosition(rigidBody2D.position + currentVelocity * Time.fixedDeltaTime);
        currentVelocity = Vector2.zero;
        velocities.Clear();
    }

    private void calculateGravity()
    {
        gravityVelocity = groundChecker.IsGrounded() ? new Vector2 (0,-0.5f) : new Vector2(0, gravityVelocity.y - gravity);
    }
    public void addVelocity(Vector2 velocity)
    {
        if (velocity != null)
            velocities.Add(velocity);
    }
    public void MoveInDirection(Vector2 inputDirection)
    {
        Vector2 groundNormal = groundChecker.GetGroundNormal();
        if (!groundChecker.IsGrounded())
        {
            movementPart = new Vector2(movementPart.x * 0.999f, movementPart.y);
            return;
        }
        Vector2 targetVelocity = surfaceSlider.projectVector(groundNormal, inputDirection);
        Vector2 velocityDifference = targetVelocity - movementPart;
        float accelerationRate = (Mathf.Abs(velocityDifference.x) > 0.01f ? accelerationPerSecond : decelerationPerSecond);
        Vector2 movementIncrement = velocityDifference.normalized * accelerationRate * Time.fixedDeltaTime;

        // Clamp the movement increment to not exceed the velocity difference
        movementIncrement = movementIncrement.magnitude > velocityDifference.magnitude ? velocityDifference : movementIncrement;
        movementPart += movementIncrement;
    }
}