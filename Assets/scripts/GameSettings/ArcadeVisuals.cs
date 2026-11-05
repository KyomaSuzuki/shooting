using UnityEngine;

// Uses the same imported sprite material as the scene, so URP keeps the shader in player builds.
public static class ArcadeVisuals
{
    private static Sprite pixel;
    public static Sprite Pixel
    {
        get
        {
            if (pixel == null) pixel = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            return pixel;
        }
    }
    public static Material SceneSpriteMaterial()
    {
        var renderer = Object.FindFirstObjectByType<SpriteRenderer>();
        return renderer == null ? null : renderer.sharedMaterial;
    }
}
