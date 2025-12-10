using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [Header("UI Settings")]
    public Slider healthBarSlider; // 体力バーのSlider

    [Header("Health Settings")]
    public float maxHealth = 100f; // インスペクタで設定できる最大体力
    private float currentHealth;

    [Header("Tag Settings")]

    // 変数名を変更: ダメージを受ける弾のタグ
    [SerializeField]
    [Tooltip("衝突するとダメージを受けるタグ一覧")]
    private string[] damageBulletTags;

    // ★追加: 回復する弾のタグ
    [SerializeField]
    [Tooltip("衝突すると回復するタグ一覧")]
    private string[] healBulletTags;

    [Header("死亡時の設定")]
    [Tooltip("体力が0になった時に表示するパネル（ゲームオーバー画面など）")]
    [SerializeField] private GameObject deathPanel;

    [Header("UI連携")] // ★追加
    [SerializeField] private PlayerHealthUI healthUI; // ★追加

    void Start()
    {
        // 初期化処理
        currentHealth = maxHealth;
        UpdateHealthBar();

        if (healthUI != null)
        {
            healthUI.InitializeHearts((int)maxHealth); // 最大体力分のハートを生成
        }
    }

    // 衝突判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string targetTag = collision.gameObject.tag;

        // 相手のオブジェクトから BulletStatus スクリプトを取得
        BulletStatus bullet = collision.gameObject.GetComponent<BulletStatus>();

        // BulletStatusを持っているか確認
        if (bullet != null)
        {
            // パターンA: ダメージタグのリストに含まれているか？
            if (IsTagInList(targetTag, damageBulletTags))
            {
                TakeDamage(bullet.damageAmount);
                Destroy(collision.gameObject); // 弾を消す
            }
            // パターンB: 回復タグのリストに含まれているか？
            else if (IsTagInList(targetTag, healBulletTags))
            {
                TakeHeal(bullet.damageAmount); // damageAmountを回復量として使う
                Destroy(collision.gameObject); // 弾を消す
            }
        }
    }

    // ダメージ処理
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (healthUI != null)
        {
            healthUI.UpdateHearts((int)currentHealth, (int)maxHealth); // ハート表示を更新
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHealthBar(); // バー更新
    }

    // 回復処理
    public void TakeHeal(float amount)
    {
        currentHealth += amount;

        // 最大体力を超えないようにする
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthBar(); // バー更新
    }

    // バーの更新処理を共通化
    private void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " dead.");
        Destroy(gameObject);

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
            Time.timeScale = 0f; // ゲームを停止
        }
    }

    // 指定したタグがリストに含まれているかチェックする汎用関数
    private bool IsTagInList(string targetTag, string[] tagsToCheck)
    {
        if (string.IsNullOrEmpty(targetTag) || tagsToCheck == null || tagsToCheck.Length == 0)
        {
            return false;
        }

        foreach (var tag in tagsToCheck)
        {
            if (!string.IsNullOrEmpty(tag) && targetTag.Equals(tag))
            {
                return true;
            }
        }
        return false;
    }
}