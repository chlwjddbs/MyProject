using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class SaveSlot : MonoBehaviour
{
    private bool isSelect = false;
    private string slotName;
    private DataManager dataManager;
    public TextMeshProUGUI savedTimeText;
    public TextMeshProUGUI gameDataText;

    private string path;

    public UserData userData = new UserData();

    private Button mButton;
    public Color nomalColor;
    public Color highlightedColor;
    public Color selectedColor;
    private ColorBlock btColor;

    private void Awake()
    {
        slotName = gameObject.name;
        mButton = GetComponent<Button>();
        btColor = mButton.colors;
        path = Application.persistentDataPath + "/Save/";
        GetComponent<Button>().onClick.AddListener(ClickSlot);
    }

    // Start is called before the first frame update
    void Start()
    {
        dataManager = DataManager.instance;
        LanguageOption.setSlot += SetSlot;
        //메인 게임에 LanguageOption 추가전까지 임시로 사용
        if (!SaveFileManager.isMain)
        {
            StartCoroutine("LoadingLanguage");
            //Debug.Log("슬롯세팅");
        }
        
        if(slotName == "AutoSave")
        {
            dataManager.GetAutoSaveSlot(this);
        }

    }

    public void SetSlot()
    {
        dataManager.LoadData(slotName,this);
        if (!File.Exists(path + slotName))
        {
            savedTimeText.text = LocalizationSettings.StringDatabase.GetLocalizedString("SaveInfo", "EmptySlot", LocalizationSettings.SelectedLocale);
            gameDataText.text = LocalizationSettings.StringDatabase.GetLocalizedString("SaveInfo", "EmptySlot", LocalizationSettings.SelectedLocale);        
            return;
        }

        //userData = dataManager.loadData;
        savedTimeText.text = userData.saveTime;
        gameDataText.text = userData.userName + " - ";

        if (slotName == "AutoSave")
        {
            gameDataText.text += LocalizationSettings.StringDatabase.GetLocalizedString("SaveInfo", "AutoSave", LocalizationSettings.SelectedLocale);
        }
        else
        {
            gameDataText.text += LocalizationSettings.StringDatabase.GetLocalizedString("SaveInfo", userData.currentStage, LocalizationSettings.SelectedLocale) + " ";
            gameDataText.text += LocalizationSettings.StringDatabase.GetLocalizedString("SaveInfo", userData.currentArea, LocalizationSettings.SelectedLocale);
        }
        Canvas.ForceUpdateCanvases();
    }

    IEnumerator LoadingLanguage()
    {
        yield return LocalizationSettings.InitializationOperation;
        SetSlot();
    }

    public void ClickSlot()
    {
        dataManager.SelectedSlot(slotName, this, userData);     
        Debug.Log(dataManager.loadData.userName);
    }
  
    public void SelectSlot()
    {
        btColor.normalColor = selectedColor; 
        btColor.highlightedColor = selectedColor;
        btColor.selectedColor = selectedColor;
        mButton.colors = btColor;
    }
    
    public void DeSelectSlot()
    {
        btColor.normalColor = nomalColor;
        btColor.highlightedColor = highlightedColor;
        btColor.selectedColor = nomalColor;
        mButton.colors = btColor;

        if (EventSystem.current.currentSelectedGameObject == mButton.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }


    public void ResetSlot()
    {
        dataManager.slotName = "";
        //dataManager.currentSlot = -1;
        userData = new UserData();
        
        savedTimeText.text = LocalizationSettings.StringDatabase.GetLocalizedString("SaveInfo", "EmptySlot");
        gameDataText.text = LocalizationSettings.StringDatabase.GetLocalizedString("SaveInfo", "EmptySlot");
    }

    public bool Loadable()
    {
        if (File.Exists(path + slotName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

/*
    private string slotName;
    private DataManager dataManager;
    public TextMeshProUGUI savedTimeText;
    public LocalizeStringEvent saveTime;
    public LocalizeStringEvent userName;
    public GameObject Hyphen;
    public LocalizeStringEvent stage;
    public LocalizeStringEvent area;

    private string path;

    public UserData userData = new UserData();

    private void Awake()
    {       
        slotName = gameObject.name;
        path = Application.persistentDataPath + "/Save/";
        //GetComponent<Button>().onClick.AddListener(SelectSlot);
        
    }
    // Start is called before the first frame update
    void Start()
    {
        dataManager = DataManager.instance;
        saveTime.StringReference.TableReference = "SaveInfo";
        userName.StringReference.TableReference = "SaveInfo";
        stage.StringReference.TableReference = "SaveInfo";
        area.StringReference.TableReference = "SaveInfo";
        SetSlot();
    }

    public void SetSlot()
    {
        //Debug.Log(slotName);
        dataManager.LoadData(slotName);
        if (!File.Exists(path + slotName))
        {
            saveTime.enabled = true;
            saveTime.StringReference.TableEntryReference = "EmptySlot";
            userName.enabled = true;
            userName.StringReference.TableEntryReference = "EmptySlot";
            EmptySlot(false);
            return;
        }

        EmptySlot(true);
        userData = dataManager.loadData;
        saveTime.enabled = false;
        savedTimeText.text = userData.saveTime;
        userName.enabled = false;
        userName.GetComponent<TextMeshProUGUI>().text = userData.userName;

        if (slotName == "AutoSave")
        {
            stage.enabled = true;
            stage.StringReference.TableEntryReference = "AutoSave";
        }
        else
        {
            LocalizationSettings.StringDatabase.GetLocalizedString("SaveInfo", userData.currentStage);
            stage.enabled = true;
            stage.StringReference.TableEntryReference = userData.currentStage;
            area.enabled = true;
            area.StringReference.TableEntryReference = userData.currentArea;
            //fileInfo.text += " - " + dataManager.userData.currentStage + "층 " + dataManager.userData.currentArea;
        }
        Canvas.ForceUpdateCanvases();
    }

    public void SelectSlot()
    {        
        dataManager.SelectedSlot(slotName,gameObject,userData);
        Debug.Log(dataManager.userData.userName);
    }

    public void EmptySlot(bool isEmpty)
    {
        Hyphen.SetActive(isEmpty);
        stage.gameObject.SetActive(isEmpty);
        area.gameObject.SetActive(isEmpty);      
    }

    public void ResetSlot()
    {
        dataManager.slotName = "";
        dataManager.currentSlot = -1;

        saveTime.enabled = true;
        saveTime.StringReference.TableEntryReference = "EmptySlot";
        userName.enabled = true;
        userName.StringReference.TableEntryReference = "EmptySlot";
        EmptySlot(false);
        //fileInfo.text = "빈 슬롯";
    }
*/