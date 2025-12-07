using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("発射設定")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 1.5f;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private float spawnOffset = 0.2f;

    private float fireTimer;

    [Header("放射状ショットの設定")]
    [Tooltip("一度に発射する弾の数")]
    public int bulletCount = 5; // 例: 3発
    [Tooltip("弾が広がる全体の角度（度数）")]
    public float spreadAngle = 60f;

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
            Debug.LogWarning("[EnemyShooting] bulletPrefab が未設定です", this);
            return;
        }

        Transform spawnPoint = firePoint != null ? firePoint : transform;

        // 1. 基準となる向き（正面）を取得
        Vector2 baseDirection = -spawnPoint.up;

        // 2. 基準の向きを「角度（度数法）」に変換
        // Atan2はラジアンを返すのでDeg2Radで度に変換
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

        // 3. 撃ち始めの角度（扇の一番左端）を計算
        // 全体の角度の半分を引くことで、正面を中心に左右対称にする
        float startAngle = baseAngle - (spreadAngle / 2f);

        // 4. 弾と弾の間の角度ステップを計算
        // 弾が1発なら角度差は0、それ以外なら (全体角度 / 間隔数)
        float angleStep = (bulletCount > 1) ? spreadAngle / (bulletCount - 1) : 0f;

        // 5. ループして弾を生成
        for (int i = 0; i < bulletCount; i++)
        {
            // A. 今回の弾の角度を計算
            float currentAngle = startAngle + (angleStep * i);

            // B. 角度からベクトル（向き）を作成
            // Mathf.Cos/Sin はラジアンを受け取るので変換が必要
            float currentAngleRad = currentAngle * Mathf.Deg2Rad;
            Vector2 bulletDir = new Vector2(Mathf.Cos(currentAngleRad), Mathf.Sin(currentAngleRad));

            // C. 弾の回転（Rotation）を作成
            // 元のコードにあった "-90f" は、スプライトの向き（上が正面）に合わせる補正と推測されます
            Quaternion rotation = Quaternion.AngleAxis(currentAngle - 90f, Vector3.forward);

            // D. 生成位置を計算（オフセットを適用）
            Vector3 spawnPosition = spawnPoint.position + (Vector3)(bulletDir * spawnOffset);

            // E. 生成
            GameObject bullet = Instantiate(prefab, spawnPosition, rotation);

            // F. 速度を適用
            Rigidbody2D rb2d = bullet.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.linearVelocity = bulletDir * bulletSpeed;
            }
        }
    }
}