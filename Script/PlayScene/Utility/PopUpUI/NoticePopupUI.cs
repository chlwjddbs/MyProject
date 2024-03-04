using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class NoticePopupUI : MonoBehaviour
{
    public LocalizeStringEvent noticeText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    public void ClosePopup()
    {
        Destroy(gameObject);
    }
}
