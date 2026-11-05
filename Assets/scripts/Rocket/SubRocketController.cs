using UnityEngine;

public class SubRocketController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public Transform rocketTransform;
    private RocketController rocketController;
    private float offset;
    private bool destroyed;
    public void SetRocketController(RocketController controller)
    {
        rocketController = controller;
        rocketTransform = controller.transform;
        offset = transform.position.x < rocketTransform.position.x ? -1f : 1f;
    }
    void Start()
    {
        if (rocketTransform == null)
        {
            var player = FindFirstObjectByType<RocketController>();
            if (player != null) SetRocketController(player);
        }
    }
    void Update()
    {
        if (rocketTransform == null) { Destroy(gameObject); return; }
        if (Time.timeScale <= 0f || (rocketController != null && rocketController.IsRunFinished)) return;
        transform.position = rocketTransform.position + Vector3.right * offset;
        if (Input.GetKeyDown(KeyCode.Space) && bulletPrefab != null)
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (destroyed || !other.CompareTag("Rock") || (rocketController != null && rocketController.IsRunFinished)) return;
        destroyed = true;
        ExplosionEffect.Spawn(explosionPrefab, transform.position);
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
