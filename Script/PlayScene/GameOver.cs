using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    private void Awake()
    {
        SaveFileManager.isMain = false;
    }
}
