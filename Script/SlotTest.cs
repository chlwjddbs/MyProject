using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class SlotTest : MonoBehaviour
{
    private string slotName;
    private DataManager dataManager;
    public TextMeshProUGUI savedTimeText;
    public TextMeshProUGUI gameDataText;

    private string path;

    public UserData userData = new UserData();

    private void Awake()
    {
        /*
        slotName = gameObject.name;
        path = Application.persistentDataPath + "/Save/";
        GetComponent<Button>().onClick.AddListener(SelectSlot); 
        */
    }
    // Start is called before the first frame update
    void Start()
    {
        dataManager = DataManager.instance;
        //LanguageOption.setSlot += SetSlot;
        //SetSlot();
    }

    /*
    public void SetSlot()
    {
        dataManager.LoadData(slotName,this.GetComponent<SaveSlot>());
        if (!File.Exists(path + slotName))
        {
            savedTimeText.text = LocalizationSettings.StringDatabase.GetLocalizedString("SaveInfo", "EmptySlot",LocalizationSettings.SelectedLocale);
            gameDataText.text = LocalizationSettings.StringDatabase.GetLocalizedString("SaveInfo", "EmptySlot", LocalizationSettings.SelectedLocale);
            
            return;
        }

        userData = dataManager.loadData;
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
    */

    /*
    public void SelectSlot()
    {
        dataManager.SelectedSlot(slotName, this ,userData);
        Debug.Log(dataManager.userData.userName);
    }
    */

    public void ResetSlot()
    {
        dataManager.slotName = "";
        //dataManager.currentSlot = -1;

        savedTimeText.text = LocalizationSettings.StringDatabase.GetLocalizedString("SaveInfo", "EmptySlot");
        gameDataText.text = LocalizationSettings.StringDatabase.GetLocalizedString("SaveInfo", "EmptySlot");       
    }
}
