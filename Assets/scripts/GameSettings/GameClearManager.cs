using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

public class GameClearManager : MonoBehaviour
{
    public GameObject gameClearScreen;
    public GameObject firstSelectedButton;
    private bool isGameClear = false;
    private int score;

    void Start()
    {
        score = GetComponent<UIController>().score;
    }

    void Update()
    {
        if (isGameClear)
        {
            return;
        }

        // ゲームクリアの条件をチェックするロジックをここに追加
        // スコアが1000を超えたらゲームクリア
        if (score >= 1000 && !this.IsGameClear())
        {
            TriggerGameClear();
        }

    }

    public void TriggerGameClear()
    {
        if (!isGameClear)
        {
            StartCoroutine(GameClearRoutine());
        }
    }

    private IEnumerator GameClearRoutine()
    {
        isGameClear = true;
        //Rocketが隕石と衝突してもゲームオーバーにならない
        RocketController rocketController = GameObject.Find("Rocket").GetComponent<RocketController>();
        rocketController.enabled = false;
        

        // 3秒待つ
        yield return new WaitForSeconds(1f);

        // 画面の動きを停止する
        Time.timeScale = 0f;

        // ゲームクリア画面を表示する
        gameClearScreen.SetActive(true);

        // 最初のボタンを選択
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void RestartGame()
    {
        // 画面の動きを再開する
        Time.timeScale = 1f;

        // 現在のシーンをリロードする
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //TitleSceneに戻る 
    public void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public bool IsGameClear()
    {
        return isGameClear;
    }
}