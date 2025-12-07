using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    // === 1回のスポーン情報を定義するクラス ===
    [System.Serializable]
    public class SpawnEntry
    {
        [Tooltip("出現させる敵のPrefab")]
        public GameObject enemyPrefab;

        [Tooltip("出現させる座標")]
        public Vector2 spawnPoint;
    }

    // === 1つのウェーブの設定を定義するクラス ===
    [System.Serializable]
    public class WaveData
    {
        [Header("ウェーブ基本設定")]
        public string waveName = "Wave";
        public float interval = 0.2f;     // このウェーブ内で敵が次々出る間隔
        public float delayAfterWave = 3.0f; // 次のウェーブに行く前の待機時間

        [Header("スポーンリスト（敵と場所の羅列）")]
        [Tooltip("上から順番に出現します")]
        public List<SpawnEntry> spawnList;
    }

    [Header("ステージ構成")]
    public List<WaveData> stageWaves = new List<WaveData>();

    void Start()
    {
        StartCoroutine(RunStage());
    }

    IEnumerator RunStage()
    {
        yield return new WaitForSeconds(5.0f);

        // ウェーブの羅列を順に実行
        foreach (var wave in stageWaves)
        {
            Debug.Log($"--- {wave.waveName} 開始 ---");

            // スポーンリスト（敵と場所のセット）を上から順に実行
            foreach (SpawnEntry entry in wave.spawnList)
            {
                // プレハブが設定されているかチェック（空欄だとエラーになるため）
                if (entry.enemyPrefab != null)
                {
                    Instantiate(entry.enemyPrefab, entry.spawnPoint, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning($"{wave.waveName} のスポーンリストにPrefabが設定されていない項目があります。");
                }

                // 次の敵が出るまでの間隔
                yield return new WaitForSeconds(wave.interval);
            }

            // --- 敵全滅待ちロジック ---

            // その後、敵が全滅するまで待機（0.5秒おきにチェック）
            while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }

            Debug.Log($"--- {wave.waveName} 完了（敵全滅） ---");

            // ウェーブ後の待機時間を消化
            yield return new WaitForSeconds(wave.delayAfterWave);
        }

        Debug.Log("--- ステージ全クリア ---");
    }

    // === ギズモ表示（可視化） ===
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (stageWaves != null)
        {
            foreach (var wave in stageWaves)
            {
                if (wave.spawnList != null)
                {
                    foreach (var entry in wave.spawnList)
                    {
                        // 座標に赤い丸を表示
                        Gizmos.DrawWireSphere(entry.spawnPoint, 0.3f);
                    }
                }
            }
        }
    }
}