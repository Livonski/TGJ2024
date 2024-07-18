using UnityEngine;
using UnityEngine.UIElements;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] float rayLength = 0.2f;
    private Transform objectTransform;
    private Collider2D objectCollider;

    [SerializeField] private float groundCasheTime;
    private float time = 0;

    private Vector2 lastGroundNormal;
    private Vector2 groundNormal;

    [SerializeField] private bool lastGrounded = true;
    [SerializeField] private bool grounded = false;

    private void Awake()
    {
        objectTransform = GetComponent<Transform>();
        objectCollider = GetComponent<Collider2D>();
    }

    public bool IsGrounded()
    {
        return lastGrounded;
    }

    public Vector2 GetGroundNormal()
    {
        return lastGroundNormal;
    }

    private void FixedUpdate()
    {
        grounded = updateGrounded();
        Debug.Log("grounded: " + grounded);
        Debug.Log("last grounded: " + lastGrounded);
        if((grounded == false) && (lastGrounded == true))
        {
            time += Time.deltaTime;
            if (time >= groundCasheTime)
            {
                lastGrounded = grounded;
                lastGroundNormal = groundNormal;
                time = 0;
            }
        }
        else
        {
            lastGrounded = grounded;
            lastGroundNormal = groundNormal;
            time = 0;
        }
        //time += ((grounded = false) && (lastGrounded = true)) ? Time.deltaTime : 0;
        //lastGrounded = time >= groundCasheTime ? grounded : lastGrounded;
        //lastGroundNormal = time >= groundCasheTime ? groundNormal : lastGroundNormal;
        //time = time >= groundCasheTime ? 0 : time;
    }

    private bool updateGrounded()
    {
        // TODO fix me - this is a terible way of making things
        Vector2 positionMiddle = new Vector2(objectTransform.position.x, objectCollider.bounds.min.y);
        Vector2 positionLeft = new Vector2(objectCollider.bounds.min.x, objectCollider.bounds.min.y);
        Vector2 positionRight = new Vector2(objectCollider.bounds.max.x, objectCollider.bounds.min.y);

        RaycastHit2D middleHit = Physics2D.Raycast(positionMiddle, Vector2.down, rayLength, groundLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(positionLeft, Vector2.down, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(positionRight, Vector2.down, rayLength, groundLayer);

        Debug.DrawRay(positionMiddle, Vector2.down * rayLength, middleHit.collider != null ? Color.green : Color.red);
        Debug.DrawRay(positionLeft, Vector2.down * rayLength, leftHit.collider != null ? Color.green : Color.red);
        Debug.DrawRay(positionRight, Vector2.down * rayLength, rightHit.collider != null ? Color.green : Color.red);

        if (middleHit.collider != null)
        {
            groundNormal = middleHit.normal;
            return true;
        }
        if (leftHit.collider != null)
        {
            groundNormal = leftHit.normal;
            return true;
        }
        if (rightHit.collider != null)
        {
            groundNormal = rightHit.normal;
            return true;
        }
        return false;
    }
}