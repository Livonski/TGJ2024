using UnityEngine;

public class GroundChecker
{
    private LayerMask groundLayer;
    private float checkDistance;
    private Transform objectTransform;
    private Collider2D objectCollider;
    private Vector2 lastGroundNormal; // To store the last detected ground normal

    // Constructor
    public GroundChecker(Transform transform, Collider2D collider, LayerMask layer, float distance)
    {
        objectTransform = transform;
        objectCollider = collider;
        groundLayer = layer;
        checkDistance = distance;
        lastGroundNormal = Vector2.up; // Default normal pointing upwards
    }

    public bool IsGrounded()
    {
        Vector2 position = new Vector2(objectTransform.position.x, objectCollider.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, checkDistance, groundLayer);
        Debug.DrawRay(position, Vector2.down * checkDistance, hit.collider != null ? Color.green : Color.red);

        if (hit.collider != null)
        {
            lastGroundNormal = hit.normal; // Update the ground normal when grounded
            return true;
        }

        return false;
    }
    public Vector2 GetGroundNormal()
    {
        return lastGroundNormal;
    }
}