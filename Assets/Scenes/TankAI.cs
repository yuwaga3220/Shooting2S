using UnityEngine;

public class TankAI : MonoBehaviour
{
    [Header("ステータス設定")]
    [SerializeField] private float rotateSpeed = 5f;    // 振り向く速さ

    [Header("攻撃設定")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 1.0f;
    [SerializeField] private float bulletSpeed = 10f;

    [Header("ターゲット設定")]
    [SerializeField] private string enemyTag = "Enemy";

    private Transform target;
    private float fireTimer;

    private void Update()
    {
        // 1. ターゲット検索
        FindClosestEnemy();

        if (target == null) return;

        // 2. 敵の方を向く（移動はしない）
        RotateTowardsTarget();

        // 3. 攻撃処理
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireInterval;
        }
    }

    // 回転処理
    private void RotateTowardsTarget()
    {
        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        // 滑らかに回転させる
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    // 発射処理
    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // 弾の向き：回転中のズレを防ぐため、今の銃口の向き(firePoint.up)ではなく
        // 「ターゲットへの正確な方向」を計算して飛ばします
        Vector2 direction = (target.position - firePoint.position).normalized;

        // 弾の画像の回転計算
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion bulletRotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        // 生成
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }
    }

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDist = Mathf.Infinity;
        Transform closestEnemy = null;
        // 最も近い敵を探す
        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestEnemy = enemy.transform;
            }
        }
        target = closestEnemy;
    }
}