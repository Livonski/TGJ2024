using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] protected int damage = 10;
    [SerializeField] protected GameObject owner; 
    [SerializeField] protected bool destroyOnImpact = true;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    protected void HandleCollision(Collision2D collision)
    {
        if (collision.gameObject != owner)
        {
            Damageable damageable = collision.gameObject.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            if (destroyOnImpact)
            {
                Destroy(gameObject);
            }
        }
    }

    public void setOwner(GameObject _owner)
    {
        owner = _owner;
    }

    public void SetDestroyOnImpact(bool value)
    {
        destroyOnImpact = value;
    }
}