using UnityEngine;
using UnityEngine.InputSystem;

public class MouseReticle : MonoBehaviour
{
    [SerializeField] private RectTransform reticleTransform;

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        // マウスが接続されていない場合は処理しない
        if (Mouse.current == null) return;

        // 【修正1】マウス位置の取得
        Vector2 mousePos = Mouse.current.position.ReadValue();
        reticleTransform.position = mousePos;

        // 【修正2】ESCキー入力
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.visible = true;
        }
        // 【修正3】左クリック入力
        else if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.visible = false;
        }
    }
}