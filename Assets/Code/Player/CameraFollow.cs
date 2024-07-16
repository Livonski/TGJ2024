using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float overshootAmount = 0.1f;

    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        Vector3 targetPosition = player.position + offset;

        Vector3 overshootPosition = targetPosition + (velocity * overshootAmount);

        transform.position = Vector3.SmoothDamp(transform.position, overshootPosition, ref velocity, smoothSpeed);
    }
}