using UnityEngine;

public class RocketController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public GameObject subRocketPrefab;
    public AudioClip BulletSounds;
    public AudioClip ExplosionSounds;
    public AudioClip ItemGetSounds;
    private GameObject leftSubRocketInstance;
    private GameObject rightSubRocketInstance;
    private GameOverManager gameOverManager;
    private UIController ui;
    private AudioSource audioSource;
    private float spreadUntil;
    private bool finished;
    public bool IsSpreadShotActive => Time.time < spreadUntil;
    public bool IsRunFinished => finished || (ui != null && ui.IsRunFinished);
    public int SupportShipCount => (leftSubRocketInstance != null ? 1 : 0) + (rightSubRocketInstance != null ? 1 : 0);
    void Start()
    {
        gameOverManager = FindFirstObjectByType<GameOverManager>();
        ui = FindFirstObjectByType<UIController>();
        audioSource = GetComponent<AudioSource>();
    }
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }
    public void Bullet(float angle)
    {
        if (IsRunFinished || Time.timeScale <= 0f || bulletPrefab == null) return;
        Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0f, 0f, angle));
    }
    public void Explode() { ExplosionEffect.Spawn(explosionPrefab, transform.position, ExplosionSounds); }
    void Update()
    {
        if (IsRunFinished || Time.timeScale <= 0f) return;
        float direction = (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x + direction * 1.5f * Time.deltaTime, -2f, 2f);
        transform.position = position;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Bullet(0f);
            if (IsSpreadShotActive) { Bullet(30f); Bullet(-30f); }
            PlaySound(BulletSounds);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsRunFinished) return;
        if (other.CompareTag("Rock"))
        {
            FinishRun();
            Explode();
            Destroy(other.gameObject);
            if (ui != null) ui.GameOver();
            if (gameOverManager != null) gameOverManager.TriggerGameOver();
            Destroy(gameObject);
        }
        else if (other.CompareTag("item_green"))
        {
            PlaySound(ItemGetSounds);
            if (ui != null) ui.AddScore(30);
            Consume(other);
        }
        else if (other.CompareTag("item_red"))
        {
            PlaySound(ItemGetSounds);
            if (subRocketPrefab != null)
            {
                if (leftSubRocketInstance == null) leftSubRocketInstance = CreateSupport(-1f);
                else if (rightSubRocketInstance == null) rightSubRocketInstance = CreateSupport(1f);
            }
            Consume(other);
        }
        else if (other.CompareTag("item_yellow"))
        {
            PlaySound(ItemGetSounds);
            spreadUntil = Time.time + 10f;
            Consume(other);
        }
    }
    private static void Consume(Collider2D item)
    {
        item.enabled = false;
        Destroy(item.gameObject);
    }
    private GameObject CreateSupport(float offset)
    {
        var ship = Instantiate(subRocketPrefab, transform.position + Vector3.right * offset, Quaternion.identity);
        var controller = ship.GetComponent<SubRocketController>();
        if (controller != null) controller.SetRocketController(this);
        return ship;
    }
    public void FinishRun()
    {
        finished = true;
        foreach (var collider in GetComponents<Collider2D>()) collider.enabled = false;
        DestroySubRockets();
    }
    public void DestroySubRockets()
    {
        if (leftSubRocketInstance != null) Destroy(leftSubRocketInstance);
        if (rightSubRocketInstance != null) Destroy(rightSubRocketInstance);
        leftSubRocketInstance = null;
        rightSubRocketInstance = null;
    }
    void OnDestroy() { DestroySubRockets(); }
}
