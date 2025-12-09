using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理に必須

public class SceneLoader : MonoBehaviour
{
    // スタート画面からステージ選択画面へ移動
    public void LoadStageSelect()
    {
        SceneManager.LoadScene("StageSelectScene");
    }

    // ステージ1（ゲーム本編）へ移動
    // ※引数でシーン名を指定できるようにすると汎用性が高まります
    public void LoadStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // タイトル画面に戻る（ゲームオーバーやクリア後など）
    public void LoadTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // ゲーム終了（PCのexe版などで動作）
    public void QuitGame()
    {
        Debug.Log("ゲーム終了"); // エディタ上では終了しないのでログを出す
        Application.Quit();
    }
}