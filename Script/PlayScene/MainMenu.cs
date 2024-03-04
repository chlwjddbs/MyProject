using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class MainMenu : MonoBehaviour
{
    //메뉴 선택시 재생할 사운드 이름
    public string selectMenu;

    //선택된 메뉴를 저장하는 공간
    protected GameObject tempMenu;

    public GameObject MainMenuObj;
   
    public GameObject settingMenu;

    protected Color32 activeColor = new Color32(50, 50, 50, 255);
    protected Color32 disableColor = new Color32(255, 255, 255, 150);

    public GameObject confirmedPopupUI;

    //public Fader fader;

    protected DataManager dataManager;

    protected GameObject popupObj;

    protected Fader fader;

    // Start is called before the first frame update
    public virtual void Start()
    {
        //Debug.Log("main");
        dataManager = DataManager.instance;
        fader = dataManager.fader;
        MenuCloseAll();       
    }

    public virtual void Update()
    {
        if (!dataManager.isSet)
        {
            return;
        }
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            continueButton.GetComponent<Button>().interactable = false;
            continueButton.GetComponentInChildren<TextMeshProUGUI>().color = disableColor;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            continueButton.GetComponent<Button>().interactable = true;
            continueButton.GetComponentInChildren<TextMeshProUGUI>().color = activeColor;
        }
        */

        //Esc를 눌렀을때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //현재 메인 화면에 있다면 게임을 종료 할것인지 물어본다.
            if(tempMenu == MainMenuObj)
            {
                QuitGameButton();
            }
            //열려 있는 메뉴창을 닫는다.
            else
            {
                MenuClose(tempMenu);
            }          
        }
    }

    public void SettingButton()
    {
        Debug.Log("게임 세팅");
        AudioManager.instance.PlayeSound(selectMenu);
        MenuOpen(settingMenu);
    }

    public virtual void QuitGameButton()
    {
        Debug.Log("게임 종료?");
        AudioManager.instance.PlayeSound(selectMenu);

        if (popupObj != null)
        {
            Destroy(popupObj);
        }
        ConfirmedPopup popup = Instantiate(confirmedPopupUI, this.gameObject.transform).GetComponent<ConfirmedPopup>();
        popupObj = popup.gameObject;
        popup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        popup.okButton.onClick.AddListener(QuitGame);
        popup.cancelButton.onClick.AddListener(popup.Cancel);
        popup.popupText.StringReference.TableReference = "General";
        popup.popupText.StringReference.TableEntryReference = "QuitGame";
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("게임 종료");
        
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
        AudioManager.instance.PlayeSound(selectMenu);
#endif
    }

    public virtual void MenuOpen(GameObject _selectMenu)
    {
        MainMenuObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920, 0);
        _selectMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        tempMenu = _selectMenu;
    }

    public virtual void MenuClose(GameObject _closeMenu)
    {
        _closeMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920, 0);
        MainMenuObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        tempMenu = MainMenuObj;
    }

    public virtual void MenuCloseAll()
    {
        settingMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920, 0);
        //settingMenu.SetActive(false);      
    }

    public void PopupClose(GameObject popup)
    {
        //popup.SetActive(false);
        Destroy(popup);
    }
}
