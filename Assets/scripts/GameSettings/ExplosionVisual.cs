using UnityEngine;

public class ExplosionVisual : MonoBehaviour
{
    private readonly SpriteRenderer[] sparks = new SpriteRenderer[12];
    private float age;
    public void Initialize(Material material)
    {
        for (int i = 0; i < sparks.Length; i++)
        {
            var child = new GameObject("Spark");
            child.transform.SetParent(transform, false);
            var renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = ArcadeVisuals.Pixel;
            if (material != null) renderer.sharedMaterial = material;
            renderer.sortingOrder = 10;
            sparks[i] = renderer;
        }
    }
    void Update()
    {
        age += Time.deltaTime;
        float progress = Mathf.Clamp01(age / 0.45f);
        for (int i = 0; i < sparks.Length; i++)
        {
            if (sparks[i] == null) continue;
            float angle = i * Mathf.PI * 2f / sparks.Length;
            var direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            sparks[i].transform.localPosition = direction * (0.1f + progress * 0.65f);
            sparks[i].transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            sparks[i].transform.localScale = new Vector3(0.17f * (1 - progress), 0.06f, 1);
            sparks[i].color = new Color(1f, 0.75f - progress * 0.4f, 0.12f, 1 - progress);
        }
        if (progress >= 1) Destroy(this);
    }
}
