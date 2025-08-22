using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public Text scoreText;       // スコア表示用テキスト
    public Text gameOverText;    // ゲームオーバー表示用テキスト
    public Text gameClearText;   // ゲームクリア表示用テキスト

    public int score = 0;       // 現在のスコア
    private GameClearManager gameClearManager;

    void Start() {
        gameClearManager = FindFirstObjectByType<GameClearManager>();

        // 初期状態でゲームオーバーとゲームクリアのテキストを非表示
        gameOverText.gameObject.SetActive(false);
        gameClearText.gameObject.SetActive(false);

        UpdateScoreUI();
    }

    public void AddScore(int points) {
        this.score += points;
        UpdateScoreUI();

        // スコアが1000を超えたらゲームクリア
        if (score >= 1000 && !gameClearManager.IsGameClear()) {
            gameClearManager.TriggerGameClear();
            DisplayGameClear();
        }
    }

    public void GameOver() {
        gameOverText.gameObject.SetActive(true);
        gameOverText.text = "GameOver\nScore:" + score.ToString("D4");
    }

    private void DisplayGameClear() {
        gameClearText.gameObject.SetActive(true);
        gameClearText.text = "GameClear\nScore:" + score.ToString("D4");
    }

    private void UpdateScoreUI() {
        scoreText.text = "Score:" + score.ToString("D4");
    }
}
