using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSceneMenu : MainMenu
{
    public GameObject loadButton;
    public GameObject saveButton;

    public SaveFileManager saveFileManager;

    public GameObject gameMenuCanvasObj;
    public static bool isMenuOpen = false;


    public override void Update()
    {
        if (!dataManager.isSet)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            /*
            if (tempMenu == null)
            {
                if (InGameUIOpenCheck())
                {
                    InGameUIClose();
                }
                else
                {
                    ToggleMenu();
                }
            }

            if (tempMenu != null)
            {
                MenuClose(tempMenu);
            }
            */

            if (InGameUIOpenCheck())
            {
                return;
            }
           
            if (tempMenu != null)
            {
                MenuClose(tempMenu);
            }
            else
            {
                ToggleMenu();
            }
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        if (isMenuOpen)
        {
            gameMenuCanvasObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            Time.timeScale = 0;
        }
        else
        {
            gameMenuCanvasObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920 , 0);
            Time.timeScale = 1;
        }
    }

    public void SetState(string _state)
    {
        AudioManager.instance.PlayeSound(selectMenu);
        saveFileManager.StateChange(_state);
        MenuOpen(saveFileManager.gameObject);
    }

    public override void MenuClose(GameObject _closeMenu)
    {
        _closeMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920, 0);
        MainMenuObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        tempMenu = null;
    }

    public override void MenuCloseAll()
    {
        base.MenuCloseAll();

        isMenuOpen = false;
        gameMenuCanvasObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920, 0);
    }

    //열려있는 UI가 있을시 UI창만 닫고
    //열려있는 UI가 없으면 Menu를 불러온다.
    public bool InGameUIOpenCheck()
    {
        if (GetComponent<InventoryUI>().UIOpenCheck()) return true;
        if (GetComponent<EquipmentUI>().UIOpenCheck()) return true;
        if (GetComponent<SkillBookUI>().UIOpenCheck()) return true;
        if (PlayerController.isCasting) return true;
        return false;
    }

    //
    public void InGameUIClose()
    {
        GetComponent<InventoryUI>().CloseUI();
        GetComponent<EquipmentUI>().CloseUI();
        GetComponent<SkillBookUI>().CloseUI();
    }

    //save와 load버튼은 SaveFileManger에서 관리중

    public void MainMenuButton()
    {
        AudioManager.instance.PlayeSound(selectMenu);

        if (popupObj != null)
        {
            Destroy(popupObj);
        }
        ConfirmedPopup popup = Instantiate(confirmedPopupUI, this.gameObject.transform).GetComponent<ConfirmedPopup>();
        popupObj = popup.gameObject;
        popup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        popup.okButton.onClick.AddListener(dataManager.GotoMainMenu);
        popup.cancelButton.onClick.AddListener(popup.Cancel);
        popup.popupText.StringReference.TableReference = "General";
        popup.popupText.StringReference.TableEntryReference = "GotoMainMenu";
    }

    public override void QuitGameButton()
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
        popup.popupText.StringReference.TableEntryReference = "QuitPlayGame";
    }
}
