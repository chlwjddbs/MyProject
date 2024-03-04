using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Localization.Components;

public class NewGamePopupUI : MonoBehaviour
{
    public TMP_InputField inputField;

    //°ø¹é ¹× Æ¯¼ö¹®ÀÚ¸¦ °É·¯³»ÁÖ´Â Á¤±Ô½Ä ÆÐÅÏ
    private string nameCheck = @"[^a-zA-Z¤¡-¤¾°¡-ÆR0-9¤¿-¤Ó]";
  
    //@"[^a-zA-Z¤¡-¤¾°¡-ÆR0-9¤¿-¤Ó]"
    //@ : Á¤±Ô½Ä Ç¥Çö ¹× ÆÄÀÏ °æ·Î ¼³Á¤°úÁ¤¿¡¼­
    //¹é½½·¹½Ã(\) ¸¦ ÀÔ·Â½Ã escape·Î µ¿ÀÛÀ» ¸ÕÀúÇÏ±â ¶§¹®¿¡ \¸¦ ¹®ÀÚ¿­·Î ¾²±â À§ÇØ¼­´Â \\ ÀÌ¶ó°í »ç¿ëÇØ¾ßÇÑ´Ù.
    //ÇÏÁö¸¸ ±×·¯¸é °¡µ¶¼ºÀÌ ¶³¾îÁö°Å³ª ºÒÆíÀ» ¾ß±âÇÏ±â ¶§¹®¿¡ @¸¦ ºÙ¿©ÁÖ¸é \¸¦ ¹®ÀÚ¿­·Î ÇÑ¹ø¿¡ ÀÎ½ÄÇÏµµ·Ï ÇØÁØ´Ù.
    //ex) @"\bAA\b" == "\\bAA\\b"

    //a-z ¼Ò¹®ÀÚ ¾ËÆÄºª
    //A-Z ´ë¹®ÀÚ ¾ËÆÄºª
    //¤¡-¤¾ ÀÚÀ½
    //¤¿-¤Ó ¸ðÀ½
    //°¡-ÆR ÇÑ±Û
    //0-9 0~9 ¸¦ ³ªÅ¸³»´Â ¼ýÀÚ

    // [] ´ë°ýÈ£¾È¿¡ ÀÖ´Â ¹®ÀÚ¸¦ Ã£´Â´Ù.
    //ex) [a-z] ¼Ò¹®ÀÚ ¾ËÆÄºªÀ» Ã£´Â´Ù. [G] ´ë¹®ÀÚ ¾ËÆÄºª G¸¦ Ã£´Â´Ù.

    //[^] ´ë°ýÈ£¾ÈÀÇ ^ »ç¿ë½Ã ºÎÁ¤À» ¶æÇÑ´Ù.
    //ex) [^a-z] ¼Ò¹®ÀÚ ¾ËÆÄºªÀ» Á¦¿ÜÇÏ°í Ã£´Â´Ù. [^G] ´ë¹®ÀÚ ¾ËÆÄºªG¸¦ Á¦¿ÜÇÏ°í Ã£´Â´Ù.

    //À§¸¦ ÀÀ¿ëÇÏ¿© [^a-zA-Z¤¡-¤¾°¡-ÆR0-9¤¿-¤Ó] Àº °ø¹é°ú Æ¯¼ö¹®ÀÚ¸¦ ¶æÇÑ´Ù.
    //´õ Á¤È®È÷´Â ¸ðµç ¾ËÆÄºª°ú ÇÑ±Û ¹× ¼ýÀÚ¸¦ Á¦¿ÜÇÑ ¹®ÀÚ¸¦ ¶æÇÑ´Ù.
    //nameCheck´Â °ø¹é ¹× Æ¯¼ö¹®ÀÚ¸¦ °É·¯ÁÖ´Â Á¤±Ô½Ä ÆÐÅÏÀÌ µÈ´Ù.

    public GameObject nameConfirmedPopupUI;

    public GameObject noticePopupUI;
    //¾È³» ÆË¾÷Ã¢À» »óÈ²¿¡ ¸Â°Ô ¾²±â À§ÇÑ LocalizeStringEvent ¼±¾ð.
    public LocalizeStringEvent noticePopupText;

    private DataManager dataManager;

    public string userName;

    
    private void Awake()
    {
        dataManager = DataManager.instance;
        //¾È³» ÆË¾÷Ã¢Àº MainMenu Å×ÀÌºí¿¡ ÀÖ´Â entry¸¦ »ç¿ë ÇÒ°Í.
        noticePopupText.StringReference.TableReference = "MainMenu";
    }

    public void SetPopup()
    {
        inputField.ActivateInputField();
        inputField.text = "";
    }

    public void OkButton()
    {
        if (inputField.text.Length == 0)
        {
            noticePopupUI.SetActive(true);
            noticePopupText.StringReference.TableEntryReference = "NameEnter";
            Debug.Log("¾ÆÀÌµð¸¦ ÀÔ·ÂÇÏ¼¼¿ä");
            return;
        }

        NameCheck();
    }

    public void NameCheck()
    {
        if (Regex.IsMatch(inputField.text, nameCheck))
        {
            Debug.Log("Æ¯¼ö ¹®ÀÚ ¹× °ø¹éÀº »ç¿ë ÇÒ ¼ö ¾ø½À´Ï´Ù.");
            noticePopupUI.SetActive(true);
            noticePopupText.StringReference.TableEntryReference = "NameNotaVailable";
        }
        else
        {
            Debug.Log("°ÔÀÓ ½ÃÀÛ");
            userName = inputField.text;
            nameConfirmedPopupUI.SetActive(true);
        }
    }

    public void CancelButton()
    {
        //Ãë¼ÒÇÏ±â´Â MainMenu.MenuClose ¸¦ ¿¬°áÇÏ¿© ±¸Çö ¿Ï·á ÇÏ¿´À½.
    }

    public void OnDisable()
    {
        nameConfirmedPopupUI.SetActive(false);
        noticePopupUI.SetActive(false);
    }
}
