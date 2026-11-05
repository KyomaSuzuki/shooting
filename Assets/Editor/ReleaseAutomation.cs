using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class ReleaseAutomation
{
    private const string Key = "SpaceShooter.ReleaseValidation";
    private static IEnumerator routine;
    private static double nextStep;
    private static double deadline;
    private static readonly List<string> checks = new List<string>();
    private static readonly List<string> errors = new List<string>();
    private static string Output => SessionState.GetString(Key + ".output", "Builds/Release");
    static ReleaseAutomation()
    {
        EditorApplication.playModeStateChanged += OnPlayMode;
        EditorApplication.update += Tick;
        EditorApplication.delayCall += CheckRequest;
    }
    private static void CheckRequest()
    {
        const string request = "Temp/run-release.flag";
        if (!File.Exists(request) || EditorApplication.isCompiling || EditorApplication.isPlaying) return;
        string output = File.ReadAllText(request).Trim();
        File.Delete(request);
        SessionState.SetString(Key + ".output", output);
        ValidateAndBuild();
    }
    [MenuItem("Tools/Space Shooter/Validate and Build Windows")]
    public static void ValidateAndBuild()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Stop Play mode first.");
        Directory.CreateDirectory(Output);
        SessionState.SetBool(Key, true);
        SessionState.SetBool(Key + ".build", false);
        EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
        EditorApplication.EnterPlaymode();
    }
    private static void OnPlayMode(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(Key, false))
        {
            checks.Clear(); errors.Clear();
            Application.logMessageReceived += OnLog;
            Application.runInBackground = true;
            deadline = EditorApplication.timeSinceStartup + 100;
            routine = Verify(); nextStep = 0;
        }
        if (state == PlayModeStateChange.EnteredEditMode && SessionState.GetBool(Key + ".build", false))
        {
            SessionState.SetBool(Key + ".build", false);
            try { BuildWindows(); }
            catch (Exception e) { File.WriteAllText(Path.Combine(Output, "build-failed.txt"), e.ToString()); Debug.LogException(e); }
        }
    }
    private static void OnLog(string message, string trace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Assert && type != LogType.Exception) return;
        if (message.Contains("MCP-FOR-UNITY") || message.Contains("Package Manager")) return;
        errors.Add(message);
    }
    private static void Tick()
    {
        if (routine == null || EditorApplication.timeSinceStartup < nextStep) return;
        try
        {
            if (EditorApplication.timeSinceStartup > deadline) throw new TimeoutException("Release validation timed out.");
            if (!routine.MoveNext()) { Finish(true); return; }
            nextStep = EditorApplication.timeSinceStartup + (routine.Current is float seconds ? seconds : 0.05f);
        }
        catch (Exception e) { errors.Add(e.ToString()); Finish(false); }
    }
    private static void Assert(bool condition, string name)
    {
        if (!condition) throw new Exception("FAIL: " + name);
        checks.Add("PASS: " + name);
    }
    private static void Pickup(RocketController rocket, string path)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        Assert(prefab != null, "Pickup prefab exists: " + path);
        var item = Object.Instantiate(prefab, new Vector3(50, 50, 0), Quaternion.identity);
        typeof(RocketController).GetMethod("OnTriggerEnter2D", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(rocket, new object[] { item.GetComponent<Collider2D>() });
    }
    private static void QuietArena()
    {
        foreach (var generator in Object.FindObjectsByType<RockGenerator>(FindObjectsSortMode.None)) generator.StopAllCoroutines();
        foreach (var generator in Object.FindObjectsByType<ItemGenerator>(FindObjectsSortMode.None)) generator.StopAllCoroutines();
        foreach (var rock in Object.FindObjectsByType<RockController>(FindObjectsSortMode.None)) Object.Destroy(rock.gameObject);
        foreach (var item in Object.FindObjectsByType<ItemController>(FindObjectsSortMode.None)) Object.Destroy(item.gameObject);
    }
    private static IEnumerator Verify()
    {
        yield return 0.5f;
        QuietArena();
        yield return 0.2f;
        Assert(Object.FindObjectsByType<UIController>(FindObjectsSortMode.None).Length == 1, "Exactly one score controller");
        var rocket = Object.FindFirstObjectByType<RocketController>();
        var ui = Object.FindFirstObjectByType<UIController>();
        Assert(rocket != null && ui != null, "Player and UI load");
        Assert(rocket.GetComponent<SpriteRenderer>().sprite != null, "Player sprite is assigned");
        foreach (var collider in rocket.GetComponents<Collider2D>()) collider.enabled = false;
        var enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/alien_ufo_0.prefab");
        var enemy = Object.Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        rocket.Bullet(0);
        var bullet = Object.FindFirstObjectByType<BulletController>();
        Assert(bullet != null, "Shot creates a projectile");
        bullet.transform.position = new Vector3(0, -0.5f, 0);
        yield return 0.5f;
        Assert(ui.score == 10 && enemy == null, "Real projectile collision destroys enemy and awards 10 once");
        ExplosionEffect.Spawn(null, Vector3.zero);
        yield return 0.1f;
        var burst = Object.FindFirstObjectByType<ExplosionVisual>();
        Assert(burst != null && burst.GetComponentsInChildren<SpriteRenderer>().All(x => x.sharedMaterial != null && x.sharedMaterial.shader.isSupported && x.sharedMaterial.shader.name != "Hidden/InternalErrorShader"), "Explosion uses a supported sprite shader");
        foreach (int rate in new[] { 30, 144 })
        {
            Application.targetFrameRate = rate;
            var moving = Object.Instantiate(enemyPrefab, new Vector3(30, 4, 0), Quaternion.identity);
            float startY = moving.transform.position.y;
            float startTime = Time.time;
            yield return 0.6f;
            float speed = (startY - moving.transform.position.y) / (Time.time - startTime);
            Assert(Mathf.Abs(speed - 0.6f) < 0.08f, "Enemy fall speed stays 0.6 units/sec at target " + rate + " fps");
            Object.Destroy(moving);
        }
        Application.targetFrameRate = -1;
        Pickup(rocket, "Assets/Prefabs/items/Item_green_0.prefab");
        Assert(ui.score == 40, "Green item awards 30");
        for (int i = 0; i < 5; i++) { Pickup(rocket, "Assets/Prefabs/items/Item_red_0.prefab"); yield return 0.1f; }
        Assert(rocket.SupportShipCount == 2 && Object.FindObjectsByType<SubRocketController>(FindObjectsSortMode.None).Length == 2, "Repeated red pickups keep exactly two support ships");
        Object.Destroy(Object.FindFirstObjectByType<SubRocketController>().gameObject);
        yield return 0.2f;
        Pickup(rocket, "Assets/Prefabs/items/Item_red_0.prefab");
        yield return 0.1f;
        Assert(rocket.SupportShipCount == 2, "Red pickup replaces a lost support ship");
        Pickup(rocket, "Assets/Prefabs/items/Item_yellow_0.prefab");
        Assert(rocket.IsSpreadShotActive, "Yellow item enables spread shot");
        yield return 1.2f;
        var expiry = typeof(RocketController).GetField("spreadUntil", BindingFlags.Instance | BindingFlags.NonPublic);
        float before = (float)expiry.GetValue(rocket);
        Pickup(rocket, "Assets/Prefabs/items/Item_yellow_0.prefab");
        Assert((float)expiry.GetValue(rocket) > before + 1f, "Repeated yellow pickup refreshes its timer");
        Time.timeScale = 0f;
        int bullets = Object.FindObjectsByType<BulletController>(FindObjectsSortMode.None).Length;
        rocket.Bullet(0);
        Assert(Object.FindObjectsByType<BulletController>(FindObjectsSortMode.None).Length == bullets, "Paused player cannot fire");
        Time.timeScale = 1f;
        yield return 10.3f;
        Assert(!rocket.IsSpreadShotActive, "Spread shot expires after ten seconds");
        ui.AddScore(960);
        var clear = Object.FindFirstObjectByType<GameClearManager>();
        var over = Object.FindFirstObjectByType<GameOverManager>();
        Assert(ui.score == 1000 && clear.IsGameClear(), "1000 points triggers game clear");
        over.TriggerGameOver(); ui.AddScore(10);
        Assert(!over.IsGameOver() && ui.score == 1000, "Clear cannot turn into game over or keep scoring");
        yield return 1.3f;
        Assert(Time.timeScale == 0f && clear.gameClearScreen.activeSelf, "Clear panel appears and freezes gameplay");
        ScreenCapture.CaptureScreenshot(Path.Combine(Output, "clear-verified.png"));
        yield return 0.2f;
        clear.RestartGame();
        yield return 0.5f;
        QuietArena();
        ui = Object.FindFirstObjectByType<UIController>();
        Assert(Time.timeScale == 1f && ui.score == 0 && !ui.IsRunFinished, "Clear restart resets the entire run");
        yield return 0.2f;
        rocket = Object.FindFirstObjectByType<RocketController>();
        Object.Instantiate(enemyPrefab, rocket.transform.position, Quaternion.identity);
        yield return 0.4f;
        over = Object.FindFirstObjectByType<GameOverManager>();
        Assert(over.IsGameOver() && rocket == null, "Real enemy collision triggers game over");
        yield return 1.2f;
        Assert(Time.timeScale == 0f && over.gameOverScreen.activeSelf, "Game over panel appears");
        over.RestartGame();
        yield return 0.5f;
        QuietArena();
        Assert(Time.timeScale == 1f && Object.FindFirstObjectByType<UIController>().score == 0, "Game over restart restores play");
        Object.FindFirstObjectByType<GameOverManager>().LoadTitleScene();
        yield return 0.5f;
        Assert(SceneManager.GetActiveScene().name == "TitleScene" && Time.timeScale == 1f, "Return to title restores time");
        Object.FindFirstObjectByType<SceneController>().LoadManualScene();
        yield return 0.5f;
        Assert(SceneManager.GetActiveScene().name == "ManualScene", "Manual navigation works");
        Object.FindFirstObjectByType<SceneController>().LoadTitleScene();
        yield return 0.5f;
        Assert(SceneManager.GetActiveScene().name == "TitleScene", "Manual return works");
        Assert(errors.Count == 0, "No runtime errors or assertions");
    }
    private static void Finish(bool success)
    {
        routine = null;
        Application.logMessageReceived -= OnLog;
        Directory.CreateDirectory(Output);
        File.WriteAllLines(Path.Combine(Output, "validation.txt"), checks.Concat(errors.Select(x => "ERROR: " + x)).Concat(new[] { success ? "RESULT: PASS" : "RESULT: FAIL" }));
        SessionState.SetBool(Key, false);
        SessionState.SetBool(Key + ".build", success);
        Time.timeScale = 1f;
        EditorApplication.ExitPlaymode();
    }
    public static void BuildWindows()
    {
        foreach (string path in AssetDatabase.FindAssets("", new[] { "Assets/Plugins/Roslyn" }).Select(AssetDatabase.GUIDToAssetPath).Where(x => x.EndsWith(".dll")))
        {
            var importer = AssetImporter.GetAtPath(path) as PluginImporter;
            if (importer == null) continue;
            importer.SetCompatibleWithAnyPlatform(false);
            importer.SetCompatibleWithEditor(true);
            importer.SaveAndReimport();
        }
        PlayerSettings.productName = "Space Shooter";
        PlayerSettings.companyName = "KyomaSuzuki";
        PlayerSettings.bundleVersion = "0.1.0";
        PlayerSettings.defaultScreenWidth = 960;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.resizableWindow = true;
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
        string folder = Path.Combine(Output, "SpaceShooter-0.1.0-Windows-x64");
        Directory.CreateDirectory(folder);
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/TitleScene.unity", "Assets/Scenes/GameScene.unity", "Assets/Scenes/ManualScene.unity" },
            locationPathName = Path.Combine(folder, "SpaceShooter.exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        });
        File.WriteAllText(Path.Combine(Output, "build-result.txt"), report.summary.result + "\nSize: " + report.summary.totalSize + "\nErrors: " + report.summary.totalErrors);
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Windows build failed.");
    }
}
