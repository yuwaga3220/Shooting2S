using System.Collections;
using UnityEngine;

public class EnemyMachineShooting : MonoBehaviour
{
    [Header("発射設定")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] public Transform firePoint1;
    [SerializeField] public Transform firePoint2;
    [SerializeField] private float fireInterval = 0.1f;
    [SerializeField] private float bulletSpeed = 8f;

    private float fireTimer;
    private Transform target;

    // 交互撃ち用のフラグ（trueならpoint1, falseならpoint2から発射）
    private bool isFiringFromPoint1 = true;

    private void Start()
    {
        fireTimer = fireInterval;
    }

    private void Update()
    {
        // ターゲット再検索ロジック
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Tank");
            if (playerObj != null) target = playerObj.transform;
        }

        fireTimer -= Time.deltaTime;

        // タイマーが0以下になり、かつターゲットがいれば発射
        if (fireTimer <= 0f)
        {
            if (target != null)
            {
                // ここで発射処理を直接呼ぶ
                FireAlternately();
            }
            // タイマーをリセット
            fireTimer = fireInterval;
        }
    }

    // 交互に発射するメソッド
    private void FireAlternately()
    {
        // フラグを見てどちらから撃つか決定
        Transform currentFirePoint = isFiringFromPoint1 ? firePoint1 : firePoint2;

        // 弾を発射
        FireOneBullet(currentFirePoint);

        // 次回のためにフラグを反転させる
        isFiringFromPoint1 = !isFiringFromPoint1;
    }

    private void FireOneBullet(Transform firePoint)
    {
        if (bulletPrefab == null) return;

        Transform spawnPoint = firePoint != null ? firePoint : transform;

        // 1. ターゲットへの方向ベクトル
        Vector2 direction = (target.position - spawnPoint.position).normalized;

        // 2. 弾の回転
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        // 3. 生成
        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, rotation);

        // 4. 速度を適用
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Unity 6以降は linearVelocity、それ以前は velocity を使用
            rb.linearVelocity = direction * bulletSpeed; 
        }
    }
}