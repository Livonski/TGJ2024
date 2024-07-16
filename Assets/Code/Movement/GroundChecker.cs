using UnityEngine;
using UnityEngine.UIElements;

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
        // TODO fix me - this is a terible way of making things
        Vector2 positionMiddle = new Vector2(objectTransform.position.x, objectCollider.bounds.min.y);
        Vector2 positionLeft = new Vector2(objectCollider.bounds.min.x, objectCollider.bounds.min.y);
        Vector2 positionRight = new Vector2(objectCollider.bounds.max.x, objectCollider.bounds.min.y);

        RaycastHit2D middleHit = Physics2D.Raycast(positionMiddle, Vector2.down, checkDistance, groundLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(positionLeft, Vector2.down, checkDistance, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(positionRight, Vector2.down, checkDistance, groundLayer);

        Debug.DrawRay(positionMiddle, Vector2.down * checkDistance, middleHit.collider != null ? Color.green : Color.red);
        Debug.DrawRay(positionLeft, Vector2.down * checkDistance, leftHit.collider != null ? Color.green : Color.red);
        Debug.DrawRay(positionRight, Vector2.down * checkDistance, rightHit.collider != null ? Color.green : Color.red);

        if (middleHit.collider != null)
        {
            lastGroundNormal = middleHit.normal; 
            return true;
        }
        if (leftHit.collider != null)
        {
            lastGroundNormal = leftHit.normal; 
            return true;
        }
        if (rightHit.collider != null)
        {
            lastGroundNormal = rightHit.normal;
            return true;
        }



        return false;
    }
    public Vector2 GetGroundNormal()
    {
        return lastGroundNormal;
    }
}