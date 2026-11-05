using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject firstSelectedButton;
    private bool isGameOver;
    public void TriggerGameOver()
    {
        var clear = FindFirstObjectByType<GameClearManager>();
        if (isGameOver || (clear != null && clear.IsGameClear())) return;
        isGameOver = true;
        StartCoroutine(GameOverRoutine());
    }
    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 0f;
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
    public void RestartGame() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void LoadTitleScene() { Time.timeScale = 1f; SceneManager.LoadScene("TitleScene"); }
    public bool IsGameOver() { return isGameOver; }
}
