using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("生成設定")]
    [Tooltip("生成する敵のプレハブ")]
    [SerializeField] private GameObject enemyPrefab;
    
    [Tooltip("敵が出現する位置（複数設定可）")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("ウェーブの基本設定")]
    [Tooltip("最初のウェーブで出現させる敵の数")]
    [SerializeField] private int initialEnemyCount = 5;
    
    [Tooltip("ウェーブが進むごとに増やす敵の数")]
    [SerializeField] private int enemyIncreasePerWave = 2;
    
    [Tooltip("敵と敵が出現する間の時間（秒）")]
    [SerializeField] private float timeBetweenSpawns = 1f;
    
    [Tooltip("ウェーブとウェーブの間の待機時間（秒）")]
    [SerializeField] private float timeBetweenWaves = 5f;

    // 現在の状態管理用
    private int currentWaveNumber = 1; // 現在のウェーブ数
    private int enemiesToSpawnInCurrentWave; // 現在のウェーブで生成予定の数

    void Start()
    {
        // 必要な設定がされているかチェック
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("WaveSpawner: Enemy Prefab または Spawn Points が設定されていません！");
            return; // 設定が足りなければ開始しない
        }

        // 最初のウェーブの敵数を設定
        enemiesToSpawnInCurrentWave = initialEnemyCount;
        
        // ウェーブ生成のコルーチンを開始
        StartCoroutine(RunWaveCycle());
    }

    // ウェーブのサイクル全体を管理するコルーチン
    private IEnumerator RunWaveCycle()
    {
        // ゲームが続いている間、無限に繰り返すループ
        // (必要に応じて終了条件を追加してください。例: while(!isGameOver))
        while (true)
        {
            Debug.Log($"--- Wave {currentWaveNumber} 開始 (予定数: {enemiesToSpawnInCurrentWave}体) ---");

            // 1. 敵を生成するフェーズ
            // 指定された数だけ敵を順番に生成する
            for (int i = 0; i < enemiesToSpawnInCurrentWave; i++)
            {
                SpawnSingleEnemy();
                // 次の敵を生成するまで待機
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            Debug.Log($"Wave {currentWaveNumber} の敵生成が完了しました。");

            // 2. 次のウェーブまでの待機フェーズ
            Debug.Log($"次のウェーブまで {timeBetweenWaves}秒 待機します...");
            yield return new WaitForSeconds(timeBetweenWaves);

            // 3. 次のウェーブの準備フェーズ
            currentWaveNumber++; // ウェーブ数を進める
            enemiesToSpawnInCurrentWave += enemyIncreasePerWave; // 次の敵の数を増やす
        }
    }

    // 敵を1体生成するメソッド
    private void SpawnSingleEnemy()
    {
        // 生成位置の配列からランダムに1つ選ぶ
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedSpawnPoint = spawnPoints[randomIndex];

        // 選んだ位置と回転で敵プレハブを実体化（生成）する
        Instantiate(enemyPrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation);
    }
}