using UnityEngine;

public class TitleCursor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     public Texture2D cursorTexture;

    void Start()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}
