using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;


public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject firstSelectedButton;
    private bool isGameOver = false;

    void Update()
    {
        if (isGameOver)
        {
            return;
        }
    }

    public void TriggerGameOver()
    {
        if (!isGameOver)
        {
            StartCoroutine(GameOverRoutine());
        }
    }

    private IEnumerator GameOverRoutine()
    {
        isGameOver = true;

        // 3秒待つ
        yield return new WaitForSeconds(3f);

        // 画面の動きを停止する
        Time.timeScale = 0f;

        // ゲームオーバー画面を表示する
        gameOverScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void RestartGame()
    {
        // 画面の動きを再開する
        Time.timeScale = 1f;

        // 現在のシーンをリロードする
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /*public void QuitGame()
    {
        // ゲームを終了する
        Application.Quit();

        // Unityエディタでは、実行停止（デバッグ用）
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }*/
    //TitleSceneに戻る 
    public void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}