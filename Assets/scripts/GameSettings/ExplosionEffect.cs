using UnityEngine;

public static class ExplosionEffect
{
    public static void Spawn(GameObject prefab, Vector3 position, AudioClip sound = null)
    {
        GameObject effect;
        if (prefab != null) effect = Object.Instantiate(prefab, position, Quaternion.identity);
        else
        {
            effect = new GameObject("Explosion");
            effect.transform.position = position;
            effect.AddComponent<ExplosionVisual>().Initialize(ArcadeVisuals.SceneSpriteMaterial());
        }
        if (sound != null)
        {
            var audio = effect.AddComponent<AudioSource>();
            audio.spatialBlend = 0f;
            audio.PlayOneShot(sound);
        }
        Object.Destroy(effect, sound == null ? 0.6f : Mathf.Max(0.6f, sound.length));
    }
}
