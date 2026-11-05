using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text scoreText;
    public Text gameOverText;
    public Text gameClearText;
    public int score;
    private GameClearManager clearManager;
    private GameOverManager overManager;
    public bool IsRunFinished => (clearManager != null && clearManager.IsGameClear()) || (overManager != null && overManager.IsGameOver());
    void Awake()
    {
        clearManager = FindFirstObjectByType<GameClearManager>();
        overManager = FindFirstObjectByType<GameOverManager>();
    }
    void Start()
    {
        if (gameOverText != null) gameOverText.gameObject.SetActive(false);
        if (gameClearText != null) gameClearText.gameObject.SetActive(false);
        UpdateScoreUI();
    }
    public void AddScore(int points)
    {
        if (IsRunFinished || points <= 0) return;
        score += points;
        UpdateScoreUI();
        if (score >= 1000 && clearManager != null)
        {
            if (gameClearText != null)
            {
                gameClearText.gameObject.SetActive(true);
                gameClearText.text = "GameClear\nScore:" + score.ToString("D4");
            }
            clearManager.TriggerGameClear();
        }
    }
    public void GameOver()
    {
        if (gameOverText == null) return;
        gameOverText.gameObject.SetActive(true);
        gameOverText.text = "GameOver\nScore:" + score.ToString("D4");
    }
    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score:" + score.ToString("D4");
    }
}
