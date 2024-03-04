using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Texture2D orginCursor;
    public Texture2D itemOverCursor_Enable;
    public Texture2D itemOverCursor_Disable;
    public Texture2D objectOverCursor;
    public Texture2D activateObjectCursor;
    public Texture2D DisabledObjectCursor;
    public Texture2D readingObjectCursor_Enable;
    public Texture2D readingObjectCursor_Disable;

    public Texture2D dumy;

    private Dictionary<string, Texture2D> cursorDictionary;

    //커서 모드
    public CursorMode cursorMode = CursorMode.Auto;

    //커서 클릭 지점
    //1920:1080 기준 X:80 Y :20 
    public Vector2 hotSpot;

    // Start is called before the first frame update
    void Start()
    {
        cursorDictionary = new Dictionary<string, Texture2D>();
        cursorDictionary.Add("orginCursor", orginCursor);
        cursorDictionary.Add("itemOverCursor_Enable", itemOverCursor_Enable);
        cursorDictionary.Add("itemOverCursor_Disable", itemOverCursor_Disable);
        cursorDictionary.Add("objectOverCursor", objectOverCursor);
        cursorDictionary.Add("activateObjectCursor", activateObjectCursor);
        cursorDictionary.Add("DisabledObjectCursor", DisabledObjectCursor);
        cursorDictionary.Add("readingObjectCursor_Enable", readingObjectCursor_Enable);
        cursorDictionary.Add("readingObjectCursor_Disable", readingObjectCursor_Disable);

        ResetCursor();
    }

    public void SetCursur(Texture2D _cursor)
    {
        if (!PlayerController.isAction)
        {
            Cursor.SetCursor(_cursor, hotSpot, cursorMode);
        }
    }
    
    public void SetCursurImage(cursorImages _cursorName)
    {
        if(!PlayerController.isAction)
        {
            Texture2D _cursor;
            if (cursorDictionary.ContainsKey(_cursorName.ToString()))
            {
                _cursor = cursorDictionary[_cursorName.ToString()];
                Cursor.SetCursor(_cursor, hotSpot, cursorMode);
            }
        }
    }
    

    public void ResetCursor()
    {
        Cursor.SetCursor(orginCursor, hotSpot, cursorMode);
    }
}

public enum cursorImages
{
    orginCursor,
    itemOverCursor_Enable,
    itemOverCursor_Disable,
    objectOverCursor,
    activateObjectCursor,
    DisabledObjectCursor,
    readingObjectCursor_Enable,
    readingObjectCursor_Disable,
}

