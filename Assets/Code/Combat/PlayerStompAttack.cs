using UnityEngine;

public class PlayerStompAttack : DamageDealer
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy" && collision.contacts[0].normal.y > 0.5)
        {
            Damageable damageable = collision.gameObject.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);

                Rigidbody2D rb = owner.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 10f);
                }
            }
        }
        //else
        //{
        //    base.OnCollisionEnter2D(collision);
        //}
    }
}