using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public string abilityName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AbilityManager.Instance.GrantAbility(abilityName);
            Destroy(gameObject);
        }
    }
}