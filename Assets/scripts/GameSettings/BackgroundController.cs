using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    void Update()
    {
        Vector3 position = transform.position;
        position.y = Mathf.Repeat(position.y + 4.9f - 1.2f * Time.deltaTime, 9.8f) - 4.9f;
        transform.position = position;
    }
}
