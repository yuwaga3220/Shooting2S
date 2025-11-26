using UnityEngine;

public class BulletStatus : MonoBehaviour
{
    // ダメージ量（敵側のTakeDamageに渡す）
    [SerializeField]
    public int damageAmount = 1;

    [SerializeField]
    [Tooltip("これらのタグを持つ相手には貫通します")]
    private string[] passThroughTags;

    /// <summary>
    /// 他のColliderと接触したときに呼ばれる
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other == null)
        {
            return;
        }

        // 指定タグには貫通させる
        if (passThroughTags != null && passThroughTags.Length > 0)
        {
            foreach (string tag in passThroughTags)
            {
                if (!string.IsNullOrEmpty(tag) && other.CompareTag(tag))
                {
                    return;
                }
            }
        }

        Destroy(gameObject);
    }
}