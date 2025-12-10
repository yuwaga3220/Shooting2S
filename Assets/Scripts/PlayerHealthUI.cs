using UnityEngine;
using UnityEngine.UI; // Image操作用（今回はGameObject操作が主ですが念のため）

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI設定")]
    [Tooltip("ハートを並べる親オブジェクト（Horizontal Layout Group付き）")]
    [SerializeField] private Transform heartContainer;

    [Header("ハートのプレハブ設定")]
    [Tooltip("体力が満タンの時のハートPrefab")]
    [SerializeField] private GameObject fullHeartPrefab;
    
    [Tooltip("体力が空（ダメージを受けた）時のハートPrefab")]
    [SerializeField] private GameObject emptyHeartPrefab;

    // 初期化時は「現在の体力」を使って表示更新するだけでOK
    public void InitializeHearts(int maxHealth)
    {
        // 初期状態（満タン）で表示
        UpdateHearts(maxHealth, maxHealth);
    }

    // 体力が変わった時に表示を更新する処理
    // 引数に maxHealth も追加して、空のハートをいくつ出すか計算できるようにします
    public void UpdateHearts(int currentHealth, int maxHealth = -1) 
    {
        // maxHealthが指定されていない（-1の）場合は、現在並んでいる数から推定するか、
        // HealthController側で必ず渡すように修正します。
        // 今回はシンプルにするため、HealthController側を修正してmaxHealthも渡すようにします。
        
        // 1. 今あるハートを全て削除してリセット
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. 「現在の体力」の分だけ、満タンハートを生成
        for (int i = 0; i < currentHealth; i++)
        {
            Instantiate(fullHeartPrefab, heartContainer);
        }

        // 3. 「減った体力」の分だけ、空のハートを生成
        // （最大体力 - 現在体力 = 空のハートの数）
        if (maxHealth > 0)
        {
            int emptyCount = maxHealth - currentHealth;
            for (int i = 0; i < emptyCount; i++)
            {
                Instantiate(emptyHeartPrefab, heartContainer);
            }
        }
    }
}