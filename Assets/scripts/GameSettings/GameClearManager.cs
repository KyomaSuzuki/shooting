using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameClearManager : MonoBehaviour
{
    public GameObject gameClearScreen;
    public GameObject firstSelectedButton;
    private bool isGameClear;
    public void TriggerGameClear()
    {
        var over = FindFirstObjectByType<GameOverManager>();
        if (isGameClear || (over != null && over.IsGameOver())) return;
        isGameClear = true;
        var rocket = FindFirstObjectByType<RocketController>();
        if (rocket != null) rocket.FinishRun();
        StartCoroutine(GameClearRoutine());
    }
    private IEnumerator GameClearRoutine()
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 0f;
        if (gameClearScreen != null) gameClearScreen.SetActive(true);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
    public void RestartGame() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void LoadTitleScene() { Time.timeScale = 1f; SceneManager.LoadScene("TitleScene"); }
    public bool IsGameClear() { return isGameClear; }
}
