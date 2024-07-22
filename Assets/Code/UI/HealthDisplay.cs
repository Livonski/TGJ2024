using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Damageable target;
    [SerializeField] private Text healthText;
    [SerializeField] private Slider healthBar;

    [SerializeField] private bool displayHealth = false;
    private void Awake()
    {
        target = GetComponent<Damageable>();
    }


    void Update()
    {
        if (healthBar == null)
            healthBar = GameObject.FindGameObjectWithTag("healthDisplay").GetComponent<Slider>();
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Damageable>();


        if (target != null && displayHealth)
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