using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private float invulnerabilityDuration = 2.0f; // Duration in seconds
    [SerializeField] private bool shakeOnImpact;
    [SerializeField] private float shakeMagnitude;
    [SerializeField] private float shakeDuration;

    private float invulnerabilityTimer = 0f;
    private bool isInvulnerable = false;

    void Update()
    {
        invulnerabilityTimer += isInvulnerable ? Time.deltaTime : 0;
        if (invulnerabilityTimer >= invulnerabilityDuration)
        {
            isInvulnerable = false;
            invulnerabilityTimer = 0f;
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isInvulnerable)
        {
            health -= amount;
            if (shakeOnImpact)
                Camera.main.GetComponent<CameraFollow>().ShakeCamera(shakeMagnitude, shakeDuration);
            if (health <= 0)
            {
                Die();
            }
            else
            {
                isInvulnerable = true;
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject); 
    }
}