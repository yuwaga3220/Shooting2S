using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    // === インスペクターで参照する敵のPrefabリスト ===

    [Header("敵のPrefab設定")]
    [Tooltip("ウェーブ1：")]
    public GameObject straightShooterPrefab;

    [Tooltip("ウェーブ2：")]
    public GameObject threeWayShooterPrefab;

    [Tooltip("ウェーブ3：")]
    public GameObject fiveWayShooterPrefab;

    // === スポーン座標の設定 ===

    // スポーン開始座標（画面上端中央付近）
    private readonly Vector2 spawnPositionTop = new Vector2(0f, 4.0f);
    // 敵を横にずらして配置するためのオフセット
    private readonly float horizontalOffset = 1.5f;

    // === ステージ管理用の変数 ===
    private int currentWave = 0;
    private const int TOTAL_WAVES = 3;

    void Start()
    {
        // 最初のウェーブを開始
        StartCoroutine(SpawnWaves());
    }

    // === ウェーブを順次実行するコルーチン ===
    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(2.0f); // ゲーム開始前の待機時間

        // ウェーブ1
        currentWave = 1;
        Debug.Log("--- ウェーブ 1 開始 ---");
        yield return StartCoroutine(SpawnWave(straightShooterPrefab, 5));

        // ウェーブ間のインターバル
        yield return new WaitForSeconds(5.0f);

        // ウェーブ2
        currentWave = 2;
        Debug.Log("--- ウェーブ 2 開始 ---");
        yield return StartCoroutine(SpawnWave(threeWayShooterPrefab, 5));

        // ウェーブ間のインターバル
        yield return new WaitForSeconds(5.0f);

        // ウェーブ3
        currentWave = 3;
        Debug.Log("--- ウェーブ 3 開始 ---");
        yield return StartCoroutine(SpawnWave(fiveWayShooterPrefab, 5));

        // 全ウェーブ終了
        Debug.Log("--- ステージ 1 完了 ---");
    }

    // === 特定のPrefabを、指定された数だけスポーンさせるコルーチン ===
    IEnumerator SpawnWave(GameObject enemyPrefab, int count)
    {
        // 敵のスポーン位置を調整するための初期X座標
        float startX = spawnPositionTop.x - (count - 1) * horizontalOffset / 2f;

        for (int i = 0; i < count; i++)
        {
            // X座標をオフセット分ずらしながらスポーン
            Vector2 spawnPos = new Vector2(startX + i * horizontalOffset, spawnPositionTop.y);

            // 敵をインスタンス化
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }

        // 全ての敵がスポーンし終わった後、このウェーブの敵が全て倒されるまで待機するロジックをここに追加
        // （例：シーン内の敵の総数をチェックするなど）

        // 今回はシンプルに一定時間待機（敵の戦闘時間を想定）
        yield return new WaitForSeconds(10.0f);
    }
}