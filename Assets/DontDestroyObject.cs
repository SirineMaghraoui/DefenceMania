using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObject : MonoBehaviour {
    public Texture2D cursorTexture;

    void Awake()
    {
        Cursor.SetCursor(cursorTexture,Vector2.zero,CursorMode.Auto);        
        DontDestroyOnLoad(this.gameObject);
    }
}
