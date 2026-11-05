using UnityEngine;

public class RockController : MonoBehaviour
{
    private float rotationSpeed;
    private bool destroyed;
    void Start() { rotationSpeed = Random.Range(300f, 480f); }
    void Update()
    {
        transform.Translate(0f, -0.6f * Time.deltaTime, 0f, Space.World);
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        if (transform.position.y < -7f) Destroy(gameObject);
    }
    public bool TryDestroy()
    {
        if (destroyed) return false;
        destroyed = true;
        foreach (var collider in GetComponents<Collider2D>()) collider.enabled = false;
        Destroy(gameObject);
        return true;
    }
}
