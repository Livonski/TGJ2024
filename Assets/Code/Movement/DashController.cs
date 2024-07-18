using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] float rayLength = 0.2f;
    private GroundChecker groundChecker;
    private Movable movable;

    [SerializeField] private AnimationCurve dashCurve;
    [SerializeField] private float dashForce = 1.0f;
    [SerializeField] private float dashDuration = 1.0f;

    [SerializeField] private int maxDashCharges;
    private int dashCharges;
    private bool dashed;
    private bool eligibleToDash;
    private bool grounded;
    private float dashTimeElapsed;

    private Vector2 facingDirection;

    private void Awake()
    {
        movable = GetComponent<Movable>();
        groundChecker = new GroundChecker(transform, GetComponent<Collider2D>(), groundLayer, rayLength);
        dashCharges = maxDashCharges;
    }

    private void FixedUpdate()
    {
        grounded = groundChecker.IsGrounded();
        dashCharges = grounded ? maxDashCharges : dashCharges;
        eligibleToDash = dashCharges > 0;
        Vector2 jumpVelocity = calculateDashVector();
        movable.addVelocity(jumpVelocity);
    }

    public bool TryDash(Vector2 direction)
    {
        if (eligibleToDash)
        {
            dashed = true;
            dashCharges--;
            facingDirection = direction;
        }
        return eligibleToDash;
    }

    public bool hasDashed()
    {
        return dashed;
    }

    private Vector2 calculateDashVector()
    {
        if (!dashed)
            return Vector2.zero;
        float horizontalVelocity = dashForce + dashCurve.Evaluate(dashTimeElapsed / dashDuration) * dashDuration;
        //Debug.Log(verticalVelocity);
        dashTimeElapsed += Time.fixedDeltaTime;
        if (dashTimeElapsed >= dashDuration)
        {
            dashed = false;
            dashTimeElapsed = 0;
        }
        horizontalVelocity = facingDirection.x > 0 ? horizontalVelocity : -horizontalVelocity;

        return new Vector2(horizontalVelocity, 0);
    }

}
