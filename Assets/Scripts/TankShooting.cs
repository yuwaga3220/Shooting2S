using UnityEngine;

public class TankShooting : MonoBehaviour
{
    [Header("発射設定")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 1.5f;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private float spawnOffset = 0.2f;

    private float fireTimer;

    private void Start()
    {
        fireTimer = fireInterval;
    }

    private void Update()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            Shoot(bulletPrefab);
            fireTimer = fireInterval;
        }
    }

    private void Shoot(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[TankShooting] bulletPrefab が未設定です", this);
            return;
        }

        Transform spawnPoint = firePoint != null ? firePoint : transform;
        Vector2 direction = spawnPoint.up;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 spawnPosition = spawnPoint.position + (Vector3)(direction.normalized * spawnOffset);
        GameObject bullet = Instantiate(prefab, spawnPosition, rotation);

        Rigidbody2D rb2d = bullet.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.linearVelocity = direction.normalized * bulletSpeed;
        }
    }
}