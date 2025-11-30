using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("移動速度を設定します。")]
    public float moveSpeed = 5.0f; // 移動速度

    private Rigidbody2D rb;

    // カメラのワールド座標での境界値を格納する変数
    [Tooltip("境界までの距離を設定します。")]
    private Vector2 screenBounds = new Vector2(5.5f, 5.6f);
    private float playerWidth;
    private float playerHeight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("PlayerMovementスクリプトは、Rigidbody2Dコンポーネントが必要です。");
            enabled = false;
            return; // 追加: nullチェック後に処理を終了
        }
        // プレイヤーのサイズをColliderから取得 (Collider2Dが必要)
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            // Playerの半分のサイズを取得
            playerWidth = playerCollider.bounds.extents.x;
            playerHeight = playerCollider.bounds.extents.y;
        }
        else
        {
            // Colliderがない場合の処理
            Debug.LogWarning("Collider2Dが見つかりません。");
        }
    }

    // 物理演算の更新はFixedUpdateで行う
    void FixedUpdate()
    {
        // ... (省略: 入力取得ロジックは変更なし) ...

        Vector2 movement = Vector2.zero;
        // ... (省略: 入力処理) ...
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                movement.y += 1f;
            if (Keyboard.current.sKey.isPressed )
                movement.y -= 1f;
            if (Keyboard.current.dKey.isPressed)
                movement.x += 1f;
            if (Keyboard.current.aKey.isPressed)
                movement.x -= 1f;
        }

        movement = movement.normalized;

        // 3. 速度の適用
        rb.linearVelocity = movement * moveSpeed;

        // 4. 画面外移動の制限ロジック（FixedUpdateの最後に追加）

        // 現在の位置を取得
        Vector3 viewPos = transform.position;

        // X軸の制限
        viewPos.x = Mathf.Clamp(viewPos.x, -screenBounds.x + playerWidth, screenBounds.x - playerWidth);

        // Y軸の制限
        viewPos.y = Mathf.Clamp(viewPos.y, -screenBounds.y + playerHeight, 0.0f);

        // 制限した位置を適用
        transform.position = viewPos;
    }
}