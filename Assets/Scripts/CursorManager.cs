using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    public Vector2 normalCursorHotspot;

    [SerializeField] private Texture2D cursorTexture2;
    public Vector2 onButtonCursorHotspot;

    void Start()
    {
    }

    public void OnButtonCursorEnter()
    {
        Cursor.SetCursor(cursorTexture, normalCursorHotspot, CursorMode.Auto);
    }

    public void OnButtonCursorExit()
    {
        Cursor.SetCursor(cursorTexture2, onButtonCursorHotspot, CursorMode.Auto);
    }

    void Update()
    {
        
    }
}
