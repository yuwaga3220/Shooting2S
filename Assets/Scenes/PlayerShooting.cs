using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    // 左クリック（Shoot）用の弾丸Prefab
    [SerializeField] private GameObject bulletPrefab1;
    // 右クリック（Shoot2）用の弾丸Prefab
    [SerializeField] private GameObject bulletPrefab2;
    // 発射位置
    [SerializeField] private Transform firePoint;
    // 弾丸の初速
    [SerializeField] private float initialBulletSpeed = 12f;
    // 弾丸の生成オフセット
    [SerializeField] private float spawnOffset;

    void Update()
    {
        // 左クリックが押されたら
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot(bulletPrefab1);
        }
        // 右クリックが押されたら
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            Shoot(bulletPrefab2);
        }
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
