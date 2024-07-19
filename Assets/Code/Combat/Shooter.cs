using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float reloadSpeed = 0.5f;

    private float reloadTimer = 0f;

    void Update()
    {
        if (reloadTimer < reloadSpeed)
        {
            reloadTimer += Time.deltaTime;
        }
    }

    public void ShootInDirection(Vector2 direction)
    {
        if (reloadTimer >= reloadSpeed)
        {
            InstantiateBullet(direction.normalized);
            reloadTimer = 0f;
        }
    }

    private void InstantiateBullet(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<DamageDealer>().setOwner(gameObject);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
            rb.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
    }
}