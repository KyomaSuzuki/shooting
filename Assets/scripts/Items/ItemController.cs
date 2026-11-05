using UnityEngine;

public class ItemController : MonoBehaviour
{
    private float rotationSpeed;
    void Start() { rotationSpeed = Random.Range(180f, 300f); }
    void Update()
    {
        transform.Translate(0f, -0.48f * Time.deltaTime, 0f, Space.World);
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        if (transform.position.y < -7f) Destroy(gameObject);
    }
}
