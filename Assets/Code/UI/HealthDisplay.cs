using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Damageable target;
    [SerializeField] private Text healthText;
    [SerializeField] private Slider healthBar;
    private void Awake()
    {
        target = GetComponent<Damageable>();
    }


    void Update()
    {
        if (target != null)
        {
            DisplayHealth();
        }
    }

    void DisplayHealth()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + target.Health;
        }

        if (healthBar != null)
        {
            healthBar.value = (float)target.Health / target.MaxHealth;
        }
    }
}