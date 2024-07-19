using UnityEngine;

public class DirectionalAttack : DamageDealer
{
    [SerializeField] private Vector2 attackDirection = new Vector2(0, -1);
    [SerializeField] private float directionThreshold = 0.5f;
    [SerializeField] private string damageTag;

    [SerializeField] private bool shakeOnImpact;
    [SerializeField] private float shakeMagnitude;
    [SerializeField] private float shakeDuration;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == damageTag)
        {
            Vector2 collisionNormal = collision.contacts[0].normal;
            float dotProduct = Vector2.Dot(attackDirection.normalized, collisionNormal);
            if (dotProduct > directionThreshold)
            {
                Damageable damageable = collision.gameObject.GetComponent<Damageable>();
                if (damageable != null)
                {
                    if (shakeOnImpact)
                        Camera.main.GetComponent<CameraFollow>().ShakeCamera(shakeMagnitude, shakeDuration);
                    damageable.TakeDamage(damage);
                    Rigidbody2D rb = owner.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, 10f);
                    }
                }
            }
        }
    }
}