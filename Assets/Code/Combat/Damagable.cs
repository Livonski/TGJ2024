using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private float invulnerabilityDuration = 2.0f; // Duration in seconds

    private float invulnerabilityTimer = 0f;
    private bool isInvulnerable = false;

    void Update()
    {
        invulnerabilityTimer = isInvulnerable ? Time.deltaTime : invulnerabilityTimer;
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