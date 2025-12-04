using UnityEngine;
using System.Collections;

public class EnemySniperShooting : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;     // 発射位置（空なら自分の位置）
    [SerializeField] private float fireInterval = 2.0f; // 攻撃行動自体の間隔
    [SerializeField] private float bulletSpeed = 8f;

    [Header("連射（バースト）設定")]
    [Tooltip("1回の攻撃で連射する弾の数")]
    public int burstCount = 3;

    [Tooltip("連射時の弾と弾の間隔（秒）")]
    public float burstInterval = 0.1f;

    private float fireTimer;
    private Transform target; // 狙う対象（プレイヤー）

    private void Start()
    {
        fireTimer = fireInterval;

        // シーン内からタンクを探してターゲットにする
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }

    private void Update()
    {
        // ターゲットが見つかっていない場合、再検索を試みる（復活時など用）
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        fireTimer -= Time.deltaTime;

        // 攻撃タイミングが来て、かつターゲットがいれば発射
        if (fireTimer <= 0f)
        {
            if (target != null)
            {
                StartCoroutine(ShootBurst());
            }
            fireTimer = fireInterval;
        }
    }

    // 連射処理を行うコルーチン
    private IEnumerator ShootBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            // 連射の途中でもターゲットがいなくなったら中断
            if (target == null) yield break;

            FireOneBullet();

            // 次の弾までの待機時間
            yield return new WaitForSeconds(burstInterval);
        }
    }

    // 1発だけ弾を撃つ処理
    private void FireOneBullet()
    {
        if (bulletPrefab == null) return;

        Transform spawnPoint = firePoint != null ? firePoint : transform;

        // 1. ターゲットへの方向ベクトルを計算（ターゲット位置 - 発射位置）
        Vector2 direction = (target.position - spawnPoint.position).normalized;

        // 2. 弾の回転を計算（弾の画像の上が正面と仮定して -90度補正）
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        // 3. 生成
        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, rotation);

        // 4. 速度を適用
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }
    }
}