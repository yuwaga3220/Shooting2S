using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Left Click Settings")]
    // 左クリック（Shoot）用の弾丸Prefab
    [SerializeField] private GameObject bulletPrefab1;
    // 左クリックの連射間隔（秒）
    [SerializeField] private float fireInterval1 = 0.1f;

    [Header("Right Click Settings")]
    // 右クリック（Shoot2）用の弾丸Prefab
    [SerializeField] private GameObject bulletPrefab2;
    // 右クリックの連射間隔（秒）
    [SerializeField] private float fireInterval2 = 0.5f;


    [Header("Shield Settings")]
    [Tooltip("シールドのPrefab")]
    [SerializeField] private GameObject shieldPrefab;
    [Tooltip("シールドを展開する対象（戦車）のTransform")]
    [SerializeField] private Transform tankTransform;
    [Tooltip("シールドの持続時間")]
    [SerializeField] private float shieldDuration = 4.0f;
    [Tooltip("シールドのクールダウン（再使用までの時間）")]
    [SerializeField] private float shieldCooldown = 10.0f;



    [Header("General Settings")]
    // 発射位置
    [SerializeField] private Transform firePoint;
    // 弾丸の初速
    [SerializeField] private float initialBulletSpeed = 12f;
    // 弾丸の生成オフセット
    [SerializeField] private float spawnOffset;

    // 次に発射可能になる時間を記録する変数
    private float nextFireTime1 = 0f;
    private float nextFireTime2 = 0f;
    private float nextShieldTime = 0f; // 次にシールドが使える時間

    void Update()
    {
        // マウスがない場合は何もしない
        if (Mouse.current == null) return;

        // --- 左クリック（押しっぱなし対応） ---
        // isPressed で「押されている間」を検知し、現在時刻が発射予定時間を過ぎているかチェック
        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime1)
        {
            Shoot(bulletPrefab1);
            // 次の発射時間を「現在時刻 + 間隔」に更新
            nextFireTime1 = Time.time + fireInterval1;
        }

        // --- 右クリック（押しっぱなし対応） ---
        if (Mouse.current.rightButton.isPressed && Time.time >= nextFireTime2)
        {
            Shoot(bulletPrefab2);
            // 次の発射時間を「現在時刻 + 間隔」に更新
            nextFireTime2 = Time.time + fireInterval2;
        }

        // --- Shiftキー（シールド） ---
        // wasPressedThisFrame: 押した瞬間だけ反応
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && Time.time >= nextShieldTime)
        {
            ActivateShield();
        }
    }

    /// <summary>
    /// シールドを展開する
    /// </summary>
    private void ActivateShield()
        {
            if (shieldPrefab == null || tankTransform == null)
            {
                Debug.LogWarning("Shield Prefab または Tank Transform が設定されていません");
                return;
            }

            // シールドを戦車の位置に生成
            GameObject shield = Instantiate(shieldPrefab, tankTransform.position, Quaternion.identity);

            // 重要: シールドを戦車の子オブジェクトにする（戦車が動いてもついていくようにする）
            shield.transform.SetParent(tankTransform);

            // 一定時間後にシールドを削除
            Destroy(shield, shieldDuration);

            // クールダウン設定
            nextShieldTime = Time.time + shieldCooldown;

            Debug.Log("シールド展開！");
        }

    /// <summary>
    /// 弾丸を発射する
    /// </summary>
    /// <param name="bulletPrefab">発射する弾丸のPrefab</param>
    private void Shoot(GameObject bulletPrefab)
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("[PlayerShooting] bulletPrefab が未設定です", this);
            return;
        }

        // 弾の生成位置を設定
        Transform spawnPoint = firePoint != null ? firePoint : transform; // firePointがnullならtransformを使用

        // マウスカーソルのワールド座標を取得（2D用、新しいInput Systemを使用）
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane));
        mouseWorldPos.z = 0f; // 2DなのでZ座標を0に

        // 発射位置からマウスカーソルへの方向を計算
        Vector2 direction = (mouseWorldPos - spawnPoint.position).normalized;

        // 方向から回転を計算
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // -90度は上向きにするため
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 弾の生成
        Vector3 spawnPosition = spawnPoint.position + (Vector3)(direction * spawnOffset);
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, rotation);

        // 弾の速度を設定
        Rigidbody2D rb2d = bullet.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.linearVelocity = direction * initialBulletSpeed;
        }
    }
}