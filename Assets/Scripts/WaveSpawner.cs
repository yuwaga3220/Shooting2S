using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    // === 1回のスポーン情報を定義するクラス ===
    [System.Serializable]
    public class SpawnEntry
    {
        public GameObject enemyPrefab;

        [Tooltip("出現位置となるオブジェクト（Transform）を指定")]
        public Transform spawnPointTransform; // ★修正: Vector2からTransformに変更
    }

    // === 小ウェーブ ===
    [System.Serializable]
    public class SubWaveData
    {
        [Tooltip("インスペクタでの識別用")]
        public string subWaveName = "Sub Wave";
        public float interval = 0.5f;
        public List<SpawnEntry> spawnList;
    }

    // === 大ウェーブ ===
    [System.Serializable]
    public class BigWaveData
    {
        [Header("大ウェーブ設定")]
        public string bigWaveName = "WAVE 1"; 

        [Tooltip("この大ウェーブに含まれる小ウェーブのリスト")]
        public List<SubWaveData> subWaves; 
    }

    [Header("UI設定")]
    [SerializeField] private GameObject waveUiPanel; 
    [SerializeField] private TextMeshProUGUI waveNameText;      
    [SerializeField] private float uiDisplayTime = 3.0f; 

    [Header("ステージ構成")]
    public List<BigWaveData> stageBigWaves = new List<BigWaveData>();

    [Header("クリア時の設定")]
    [SerializeField] private GameObject clearPanel;

    void Start()
    {
        if (waveUiPanel != null) waveUiPanel.SetActive(false);
        StartCoroutine(RunStage());
    }

    IEnumerator RunStage()
    {
        yield return new WaitForSeconds(1.0f);

        // --- 外側のループ：大ウェーブ ---
        foreach (var bigWave in stageBigWaves)
        {
            // UI表示
            if (waveUiPanel != null && waveNameText != null)
            {
                waveNameText.text = bigWave.bigWaveName; 
                waveUiPanel.SetActive(true);            
                yield return new WaitForSeconds(uiDisplayTime);
                waveUiPanel.SetActive(false);           
            }

            Debug.Log($"=== {bigWave.bigWaveName} スタート ===");

            // --- 内側のループ：小ウェーブ ---
            foreach (var subWave in bigWave.subWaves)
            {
                foreach (SpawnEntry entry in subWave.spawnList)
                {
                    // Prefabと出現位置の両方が設定されているかチェック
                    if (entry.enemyPrefab != null && entry.spawnPointTransform != null)
                    {
                        // ★修正: Transformの位置(position)を使って生成
                        Instantiate(entry.enemyPrefab, entry.spawnPointTransform.position, Quaternion.identity);
                    }
                    else
                    {
                        Debug.LogWarning($"PrefabまたはSpawnPointが設定されていません: {subWave.subWaveName}");
                    }

                    yield return new WaitForSeconds(subWave.interval);
                }

                // 敵全滅待ち
                while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                {
                    yield return new WaitForSeconds(0.5f);
                }

                yield return new WaitForSeconds(1.0f);
            }

            Debug.Log($"=== {bigWave.bigWaveName} クリア ===");
            yield return new WaitForSeconds(2.0f);
        }

        Debug.Log("--- 全ステージクリア ---");
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
            Time.timeScale = 0f; // ゲームを停止
        }

    }

    // === ギズモ表示 ===
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (stageBigWaves != null)
        {
            foreach (var bigWave in stageBigWaves)
            {
                if (bigWave.subWaves != null)
                {
                    foreach(var sub in bigWave.subWaves)
                    {
                        if(sub.spawnList != null)
                        {
                            foreach (var entry in sub.spawnList)
                            {
                                // ★修正: Transformが入っている場合のみ表示
                                if (entry.spawnPointTransform != null)
                                {
                                    Gizmos.DrawWireSphere(entry.spawnPointTransform.position, 0.3f);
                                    // どの位置かわかりやすくするために線も引く
                                    Gizmos.DrawLine(transform.position, entry.spawnPointTransform.position);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}