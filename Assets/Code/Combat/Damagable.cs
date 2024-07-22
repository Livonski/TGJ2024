using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private int shieldHealth = 50; 
    [SerializeField] private float shieldDuration = 10.0f; 

    [SerializeField] private float invulnerabilityDuration = 2.0f;

    [SerializeField] private bool shakeOnImpact;
    [SerializeField] private float shakeMagnitude;
    [SerializeField] private float shakeDuration;

    private float invulnerabilityTimer = 0f;
    private float shieldTimer = 0f;
    private bool isInvulnerable = false;
    private bool shieldActive = false;

    public int Health => health;
    public int MaxHealth => maxHealth;
    public bool ShieldActive => shieldActive;

    [SerializeField] private bool displayHealth;
    public delegate void HealthChanged();
    public event HealthChanged OnHealthChanged;

    [SerializeField] private bool isPlayer = false;

    void Update()
    {
        invulnerabilityTimer += isInvulnerable ? Time.deltaTime : 0;
        if (invulnerabilityTimer >= invulnerabilityDuration)
        {
            isInvulnerable = false;
            invulnerabilityTimer = 0f;
        }

        if (shieldActive)
        {
            shieldTimer += Time.deltaTime;
            if (shieldTimer >= shieldDuration)
            {
                shieldActive = false;
                shieldTimer = 0f;
                health -= shieldHealth;
                health = Mathf.Clamp(health, 0, maxHealth);
                if (displayHealth)
                    OnHealthChanged?.Invoke();
            }
        }
    }

    public bool TryActivateShield()
    {
        if (!shieldActive)
        {
            shieldActive = true;
            health += shieldHealth;
            health = Mathf.Clamp(health, 0, maxHealth + shieldHealth); // Allow temporary health increase beyond maxHealth
            if (displayHealth)
                OnHealthChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void TakeDamage(int amount)
    {
        if (!isInvulnerable)
        {
            health -= amount;
            health = Mathf.Clamp(health, 0, shieldActive ? maxHealth + shieldHealth : maxHealth);
            if (displayHealth)
                OnHealthChanged?.Invoke();
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
        if (isPlayer)
            FindObjectOfType<LevelController>().reload();
        Destroy(gameObject); 
    }
}