using UnityEngine;

// 移動（回避）のためにRigidbody2Dが必要になります
[RequireComponent(typeof(Rigidbody2D))]
public class TankAI : MonoBehaviour
{
    [Header("ステータス設定")]
    [SerializeField] private float rotateSpeed = 5f;    // 振り向く速さ

    [Header("回避設定")] // ★追加
    [SerializeField] private string bulletTag = "EnemyBullet"; // 避ける対象のタグ
    [SerializeField] private float detectionRadius = 3.0f;     // 弾を検知する範囲（半径）
    [SerializeField] private float dodgeSpeed = 4.0f;          // 避ける移動速度

    [Header("攻撃設定")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 1.0f;
    [SerializeField] private float bulletSpeed = 10f;

    [Header("ターゲット設定")]
    [SerializeField] private string enemyTag = "Enemy";

    private Transform target;
    private float fireTimer;
    private Rigidbody2D rb;

    // カメラのワールド座標での境界値を格納する変数
    [Tooltip("境界までの距離を設定します。")]
    private Vector2 screenBounds = new Vector2(5.5f, 3.0f);
    private float playerWidth;
    private float playerHeight;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // AIの戦車自体が物理演算で回転してしまわないようにZ軸回転を固定（推奨）
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerWidth = playerCollider.bounds.extents.x;
            playerHeight = playerCollider.bounds.extents.y;
        }
        else
        {
            Debug.LogWarning("Collider2Dが見つかりません。");
        }
    }

    private void Update()
    {
        // 1. ターゲット検索
        FindClosestEnemy();

        // 2. 敵の方を向く（ターゲットがいれば）
        if (target != null)
        {
            RotateTowardsTarget();

            // 3. 攻撃処理
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = fireInterval;
            }
        }
    }

    private void FixedUpdate()
    {
        AvoidBullets();

        // ---  画面外移動の制限 ---
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, -screenBounds.x + playerWidth, screenBounds.x - playerWidth);
        viewPos.y = Mathf.Clamp(viewPos.y, -screenBounds.y + playerHeight, 0.0f);
        transform.position = viewPos;
    }

    // 弾を避ける処理
    private void AvoidBullets()
    {
        // 自分の周囲(detectionRadius)にあるコライダーを全て取得
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        Vector2 avoidVector = Vector2.zero;
        int threatCount = 0;

        foreach (Collider2D hit in hits)
        {
            // 見つけたものが「敵の弾」だったら
            if (hit.CompareTag(bulletTag))
            {
                // 「自分 - 弾」のベクトル＝弾から遠ざかる方向
                Vector2 directionAway = (transform.position - hit.transform.position).normalized;

                // 複数の弾がある場合、すべての逃げる方向を足し合わせる
                avoidVector += directionAway;
                threatCount++;
            }
        }

        if (threatCount > 0)
        {
            // 逃げる方向へ移動
            // 平均化して正規化
            avoidVector = avoidVector.normalized;
            rb.linearVelocity = avoidVector * dodgeSpeed;
        }
        else
        {
            // 危険がないときは停止（慣性で滑らないように）
            rb.linearVelocity = Vector2.zero;
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

        // 弾の精度向上のための予測計算（以前のコード維持）
        Vector2 direction = (target.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion bulletRotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb != null)
        {
            bulletRb.linearVelocity = direction * bulletSpeed;
        }
    }

    private void FindClosestEnemy()
    {
        // ※注意: Update内でFindGameObjectsWithTagを使うのは処理負荷が高いため、
        // 本格的な開発ではコルーチンで定期実行するか、リスト管理に変更することをお勧めします。
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDist = Mathf.Infinity;
        Transform closestEnemy = null;

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

    // エディタ上で検知範囲を可視化（デバッグ用）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}