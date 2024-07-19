using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float overshootAmount = 0.1f;

    private Vector3 velocity = Vector3.zero;
    private float shakeMagnitude;
    private float shakeDuration;
    private float shakeTimer;

    void FixedUpdate()
    {
        Vector3 finalPosition = CalculatePosition();
        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref velocity, smoothSpeed);
    }

    private Vector3 CalculatePosition()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            return GetShakenPosition();
        }
        return GetFollowingPosition();
    }

    private Vector3 GetShakenPosition()
    {
        Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
        shakeOffset.z = 0;  // Preserve the camera's fixed z position
        return new Vector3(player.position.x, player.position.y, player.position.z) + offset + shakeOffset;
    }

    private Vector3 GetFollowingPosition()
    {
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y, player.position.z) + offset;
        Vector3 overshootPosition = targetPosition + (velocity * overshootAmount);
        return overshootPosition;
    }

    public void ShakeCamera(float magnitude, float duration)
    {
        shakeMagnitude = magnitude;
        shakeDuration = duration;
        shakeTimer = duration;
    }
}