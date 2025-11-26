using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("移動速度を設定します。")]
    public float moveSpeed = 5.0f; // 移動速度

    private Rigidbody2D rb;

    void Start()
    {
        // アタッチされているRigidbody2Dコンポーネントを取得
        rb = GetComponent<Rigidbody2D>();

        // Rigidbody2Dがアタッチされているか確認
        if (rb == null)
        {
            Debug.LogError("PlayerMovementスクリプトは、Rigidbody2Dコンポーネントが必要です。");
            enabled = false; // Rigidbodyがない場合、スクリプトを無効にする
        }
    }

    // 物理演算の更新はFixedUpdateで行う
    void FixedUpdate()
    {
        // 1. 入力の取得（新しいInput Systemを使用）
        Vector2 movement = Vector2.zero;
        
        if (Keyboard.current != null)
        {
            // WASDまたは矢印キーからの水平・垂直入力を取得
            if (Keyboard.current.wKey.isPressed)
                movement.y += 1f;
            if (Keyboard.current.sKey.isPressed )
                movement.y -= 1f;
            if (Keyboard.current.dKey.isPressed)
                movement.x += 1f;
            if (Keyboard.current.aKey.isPressed)
                movement.x -= 1f;
        }

        // 2. 移動ベクトルの正規化
        movement = movement.normalized;
        // .normalized を付けることで、斜め移動（XとYが1の場合）の速度が速くなりすぎるのを防ぎます。

        // 3. 速度の適用
        // Rigidbody2Dの速度を直接設定して移動
        rb.linearVelocity = movement * moveSpeed;
    }
}