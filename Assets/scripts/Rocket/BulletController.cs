using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject explosionPrefab;
    private UIController ui;
    private bool spent;
    void Start()
    {
        ui = FindFirstObjectByType<UIController>();
        var visual = GetComponent<SpriteRenderer>();
        if (visual != null) { visual.sprite = ArcadeVisuals.Pixel; visual.color = new Color(0.5f, 1f, 1f); visual.sortingOrder = 5; }
        transform.localScale = new Vector3(0.1f, 0.34f, 1f);
        var trail = GetComponent<TrailRenderer>();
        if (trail != null) trail.enabled = false;
    }
    void Update()
    {
        if (ui != null && ui.IsRunFinished) { Destroy(gameObject); return; }
        transform.Translate(0f, 12f * Time.deltaTime, 0f);
        if (transform.position.y > 8f || Mathf.Abs(transform.position.x) > 8f) Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (spent || !other.CompareTag("Rock") || (ui != null && ui.IsRunFinished)) return;
        var rock = other.GetComponent<RockController>();
        if (rock != null && !rock.TryDestroy()) return;
        spent = true;
        if (rock == null) Destroy(other.gameObject);
        if (ui != null) ui.AddScore(10);
        ExplosionEffect.Spawn(explosionPrefab, transform.position);
        Destroy(gameObject);
    }
}
