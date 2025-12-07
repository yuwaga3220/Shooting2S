using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("移動速度を設定します。")]
    public float moveSpeed = 5.0f; // 移動速度

    [Tooltip("回転速度を設定します。大きいほど速く向きます。")]
    public float rotationSpeed = 200f; // ★追加: 回転速度

    private Rigidbody2D rb;
    private Camera mainCamera; // カメラの参照をキャッシュ

    // カメラのワールド座標での境界値を格納する変数
    [Tooltip("境界までの距離を設定します。")]
    private Vector2 screenBounds = new Vector2(5.5f, 5.0f);
    private float playerWidth;
    private float playerHeight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main; // カメラを取得

        if (rb == null)
        {
            Debug.LogError("PlayerMovementスクリプトは、Rigidbody2Dコンポーネントが必要です。");
            enabled = false;
            return;
        }

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

    void FixedUpdate()
    {
        // --- 1. 移動処理 ---
        Vector2 movement = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) movement.y += 1f;
            if (Keyboard.current.sKey.isPressed) movement.y -= 1f;
            if (Keyboard.current.dKey.isPressed) movement.x += 1f;
            if (Keyboard.current.aKey.isPressed) movement.x -= 1f;
        }

        movement = movement.normalized;

        rb.linearVelocity = movement * moveSpeed;


        // --- 2. 回転処理（マウスの方を向く） ---
        RotateTowardsMouse();


        // --- 3. 画面外移動の制限 ---
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, -screenBounds.x + playerWidth, screenBounds.x - playerWidth);
        viewPos.y = Mathf.Clamp(viewPos.y, -screenBounds.y + playerHeight, 0.0f);
        transform.position = viewPos;
    }

    // マウスの方向へ滑らかに回転させる処理
    void RotateTowardsMouse()
    {
        if (Mouse.current == null || mainCamera == null) return;

        // マウスの位置（スクリーン座標）を取得
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        // スクリーン座標をワールド座標に変換
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = transform.position.z; // Z軸はプレイヤーに合わせる

        // プレイヤーからマウスへの方向ベクトル
        Vector3 direction = mouseWorldPosition - transform.position;

        // 方向ベクトルから角度（ラジアン）を計算し、度数（デグリー）に変換
        // シューティングゲームの自機画像は通常「上」を向いていることが多いため、-90度して調整します
        // ※もし画像が「右」を向いているなら、-90f は削除してください
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // 目標の角度を作成
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // 現在の角度から目標の角度へ、指定した速度(rotationSpeed)で回転させる
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}