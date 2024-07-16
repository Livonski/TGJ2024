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

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] float rayLength = 0.5f;
    private Rigidbody2D rigidBody2D;
    private GroundChecker groundChecker;

    [SerializeField] private float gravity = 1.0f;

    [SerializeField] private AnimationCurve dashCurve;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    private float dashTimeElapsed;

    private SurfaceSlider surfaceSlider;
    private JumpController jumpController;

    private Vector2 currentVelocity;
    private List<Vector2> velocities;
    private Vector2 movementPart;

    private bool moved = false;
    private bool jumped = false;
    private bool dashed = false;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        jumpController = GetComponent<JumpController>();

        groundChecker = new GroundChecker(transform, GetComponent<Collider2D>(), groundLayer, rayLength);
        surfaceSlider = new SurfaceSlider(maxSpeed);

        accelerationPerSecond = maxSpeed / accelerationTime;
        decelerationPerSecond = maxSpeed / decelerationTime;

        velocities = new List<Vector2>();
    }

    private void FixedUpdate()
    {
        applyVelocity();
    }

    /*private void applyVelocity()
    {
        bool grounded = groundChecker.IsGrounded();
        if (grounded & !isJumping)
        {
            jumpPart = new Vector2(0, -1f);
        }
        else
        {
            jumpPart += isJumping ? calculateJumpVector() : new Vector2(0, -gravity);
        }
        // jumpPart.y = Mathf.Clamp(jumpPart.y,-gravity, float.MaxValue);
        // gravityPart = isJumping ? Vector2.zero : new Vector2(0, -gravity);

        //Debug.Log("jump part : " + jumpPart);

        dashPart = (dashed && !grounded) ? calculateDashVelocity() : Vector2.zero;
        Debug.Log(dashPart);
        currentVelocity = movementPart + jumpPart + dashPart;
        rigidBody2D.MovePosition(rigidBody2D.position + currentVelocity * Time.fixedDeltaTime);
    }*/
    private void applyVelocity()
    {
        foreach (Vector2 v in velocities)
        {
            currentVelocity += v;
        }
        currentVelocity += (!dashed && !jumpController.hasJumped()) ? new Vector2(0, -gravity) : Vector2.zero;
        currentVelocity += movementPart;
        //Debug.Log("movement part : " + movementPart + "current velocity : " + currentVelocity);
        rigidBody2D.MovePosition(rigidBody2D.position + currentVelocity * Time.fixedDeltaTime);
        currentVelocity = Vector2.zero;
        velocities.Clear();
        //moved = false;
        dashed = false;
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
        float accelerationRate = Mathf.Abs(velocityDifference.x) > 0.01f ? accelerationPerSecond : decelerationPerSecond;
        Vector2 movementIncrement = velocityDifference.normalized * accelerationRate * Time.fixedDeltaTime;

        // Clamp the movement increment to not exceed the velocity difference
        movementIncrement = movementIncrement.magnitude > velocityDifference.magnitude ? velocityDifference : movementIncrement;
        movementPart += movementIncrement;
        //Debug.Log(movementPart);
        //if (!moved)
        //    velocities.Add(movementPart);
        //moved = true;
    }

    private Vector2 calculateDashVelocity()
    {
        float horizontalVelocity = dashForce * dashCurve.Evaluate(dashTimeElapsed / dashDuration) * dashDuration;
        //Debug.Log(horizontalVelocity);
        dashTimeElapsed += Time.fixedDeltaTime;
        if (dashTimeElapsed >= dashDuration)
            dashed = false;
        return new Vector2(horizontalVelocity, 0);
    }

    public void Dash()
    {
        if(!dashed)
        {
            dashed = true;
            dashTimeElapsed = 0;
        }
    }
}