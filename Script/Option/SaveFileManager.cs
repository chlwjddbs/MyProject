using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using System;
using UnityEngine.SceneManagement;

public class SaveFileManager : MonoBehaviour
{
    private bool isOpen = false;

    public Button deleteButton;

    public Button loadButton;

    public Button saveButton;

    private RectTransform rect;

    public GameObject confirmedPopupUI;

    public GameObject NoticePopupUI;

    //현재 씬에 있는 objectManger : 인스펙터에서 직접 할당해준다.
    public ObjectManager objectManager;

    public static bool isMain = true;

    private DataManager dataManager;

    public LocalizeStringEvent gameSlotTitle;

    private GameObject popupObj;

    // Start is called before the first frame update

    void Start()
    {
        rect = GetComponent<RectTransform>();
        //deleteButton.onClick.AddListener(DataManager.instance.DeleteData);
        dataManager = DataManager.instance;
        gameSlotTitle.StringReference.TableReference = "General";
    }

    private void Update()
    {
        if (!dataManager.isSet)
        {
            return;
        }

        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseButton();
            }
        }
    }

    public void Toggle()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            Debug.Log("열어");
        }
        else
        {
            CloseButton();
            Debug.Log("닫아");
        }
    }

    public void CloseButton()
    {
        isOpen = false;
        try
        {
            dataManager.selectedSlot.DeSelectSlot();
        }
        catch
        {
            Debug.Log("선택된 슬롯 없음");
        }
        dataManager.ResetSlotData();
    }

    public void DeleteButton()
    {
        if (popupObj != null)
        {
            Destroy(popupObj);
        }

        if (dataManager.DeleteButtonSelect())
        {
            ConfirmedPopup popup = Instantiate(confirmedPopupUI, this.gameObject.transform).GetComponent<ConfirmedPopup>();
            popup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            //Debug.Log("팝업창 생성");
            popup.okButton.onClick.AddListener(dataManager.DeleteData);
            popup.cancelButton.onClick.AddListener(popup.Cancel);
            popup.popupText.StringReference.TableReference = "General";
            popup.popupText.StringReference.TableEntryReference = "DeleteSaveData";
            popupObj = popup.gameObject;
            //popup.GetComponent<ConfirmedPopup>().okButton.onClick.AddListener(DataManager.instance.DeleteData);
            //popup.GetComponent<ConfirmedPopup>().cancelButton.onClick.AddListener(popup.GetComponent<ConfirmedPopup>().Cancel);
        }

    }

    public void SaveButton()
    {
        if (popupObj != null)
        {
            Destroy(popupObj);
        }

        if (dataManager.selectedSlot == null)
        {
            NoticePopupUI noticePopup = Instantiate(NoticePopupUI, this.gameObject.transform).GetComponent<NoticePopupUI>();
            noticePopup.noticeText.StringReference.TableReference = "PopupUI";
            noticePopup.noticeText.StringReference.TableEntryReference = "NoSlot";
            popupObj = noticePopup.gameObject;
            Debug.Log("선택된 슬롯 없음");
            return;
        }

        ConfirmedPopup popup = Instantiate(confirmedPopupUI, this.gameObject.transform).GetComponent<ConfirmedPopup>();
        popup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        popup.okButton.onClick.AddListener(SaveGame);
        popup.cancelButton.onClick.AddListener(popup.Cancel);
        popup.popupText.StringReference.TableReference = "General";
        popupObj = popup.gameObject;

        if (dataManager.SaveButtonEmpty())
        {
            popup.popupText.StringReference.TableEntryReference = "SaveGameData";
        }
        else
        {
            popup.popupText.StringReference.TableEntryReference = "OverwriteSaveData";
        }
    }

    public void SaveGame()
    {
        Debug.Log(dataManager.userData.playerPos);      
        dataManager.userData.saveTime = DateTime.Now.ToString("yyyy-MM-dd [HH:mm]");
        dataManager.userData.sceneName = SceneManager.GetActiveScene().name;
        objectManager?.SaveData();
        Debug.Log(DataManager.instance.userData.userName);
        DataManager.instance.CreateSaveData();
    }

    public void LoadButton()
    {
        if (popupObj != null)
        {
            Destroy(popupObj);
        }

        if (dataManager.selectedSlot == null)
        {
            NoticePopupUI noticePopup = Instantiate(NoticePopupUI, this.gameObject.transform).GetComponent<NoticePopupUI>();
            noticePopup.noticeText.StringReference.TableReference = "PopupUI";
            noticePopup.noticeText.StringReference.TableEntryReference = "NoSlot";
            popupObj = noticePopup.gameObject;
            Debug.Log("선택된 슬롯 없음");
            return;
        }

        if (dataManager.selectedSlot.GetComponent<SaveSlot>().Loadable())
        {
            ConfirmedPopup popup = Instantiate(confirmedPopupUI, this.gameObject.transform).GetComponent<ConfirmedPopup>();
            popup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            popup.okButton.onClick.AddListener(dataManager.LoadGame);
            popup.cancelButton.onClick.AddListener(popup.Cancel);
            popup.popupText.StringReference.TableReference = "General";
            popup.popupText.StringReference.TableEntryReference = "LoadGameData";
            popupObj = popup.gameObject;
        }
        else
        {
            NoticePopupUI noticePopup = Instantiate(NoticePopupUI, this.gameObject.transform).GetComponent<NoticePopupUI>();
            noticePopup.noticeText.StringReference.TableReference = "PopupUI";
            noticePopup.noticeText.StringReference.TableEntryReference = "EmptySlot";
            popupObj = noticePopup.gameObject;
        }
    }

    public void MenuSceneLoadButton()
    {
        if (popupObj != null)
        {
            Destroy(popupObj);
        }

        if (dataManager.selectedSlot == null)
        {
            NoticePopupUI noticePopup = Instantiate(NoticePopupUI, this.gameObject.transform).GetComponent<NoticePopupUI>();
            noticePopup.noticeText.StringReference.TableReference = "PopupUI";
            noticePopup.noticeText.StringReference.TableEntryReference = "NoSlot";
            popupObj = noticePopup.gameObject;
            Debug.Log("선택된 슬롯 없음");
            return;
        }

        if (dataManager.selectedSlot.GetComponent<SaveSlot>().Loadable())
        {
            dataManager.LoadGame();
        }
        else
        {
            NoticePopupUI noticePopup = Instantiate(NoticePopupUI, this.gameObject.transform).GetComponent<NoticePopupUI>();
            noticePopup.noticeText.StringReference.TableReference = "PopupUI";
            noticePopup.noticeText.StringReference.TableEntryReference = "EmptySlot";
            popupObj = noticePopup.gameObject;
        }
    }

    public void GotoMenu()
    {
        dataManager.GotoMainMenu();
    }

    public void StateChange(string _state)
    {
        if(_state == "Load")
        {
            gameSlotTitle.StringReference.TableEntryReference = "LoadGame";
            saveButton.gameObject.SetActive(false);
            loadButton.gameObject.SetActive(true);
        }
        else if(_state == "Save")
        {
            gameSlotTitle.StringReference.TableEntryReference = "SaveGame";
            loadButton.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log(_state + " 는 잘못된 요청입니다.");
        }

        isOpen = true;
    }
}
