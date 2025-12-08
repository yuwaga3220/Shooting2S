using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("横に揺れる幅（振幅）")]
    [SerializeField] private float width = 3.0f;

    [Tooltip("揺れる速さ")]
    [SerializeField] private float speed = 2.0f;

    // 開始時の位置を保存しておく変数
    private Vector3 startPos;

    void Start()
    {
        // ゲーム開始時の位置を基準点として記憶する
        startPos = transform.position;
    }

    void Update()
    {
        // 三角関数(Sin)を使って横方向のズレを計算
        float x = width * Mathf.Sin(Time.time * speed);

        // 計算したX座標を適用
        transform.position = new Vector3(startPos.x + x, transform.position.y, transform.position.z);
    }
}