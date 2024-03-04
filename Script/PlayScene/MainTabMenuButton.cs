using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class MainTabMenuButton : MonoBehaviour
{
    public GameObject menuContent;
    public bool isFirstMenu = false;
    private Button mButton;

    public TextMeshProUGUI menuGuideMessage;
    public string setGuideMessage;

    public string entryReference;
    public  LocalizedString menuGuide = new LocalizedString { TableReference = "MenuGuideText", TableEntryReference = "VideoSetting"};

    public bool isSelect= false;

    public Color nomalColor;
    public Color selectedColor;
    public Color highlightedColor;
    private ColorBlock btColor;

    private void Awake()
    {
        mButton = GetComponent<Button>();
        mButton.onClick.AddListener(SelectMenu);
        btColor = mButton.colors;
        menuGuide.TableEntryReference = entryReference;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isFirstMenu)
        {           
            isSelect = true;
            SelectMenu();
        }
    }

    public void SelectMenu()
    {
        //Debug.Log(mButton.name);
        mButton.transform.parent.BroadcastMessage("DeslectMenu");
        menuContent.SetActive(true);
        //btColor = mButton.colors;
        btColor.normalColor = selectedColor;
        btColor.highlightedColor = selectedColor;
        mButton.colors = btColor;
        isSelect = true;
        
        //menuGuideMessage.text = setGuideMessage;
        menuGuideMessage.gameObject.GetComponent<LocalizeStringEvent>().StringReference = menuGuide;
    }

    public void DeslectMenu()
    {
        //btColor = mButton.colors;
        btColor.normalColor = nomalColor;
        btColor.highlightedColor = highlightedColor;
        mButton.colors = btColor;
        isSelect = false;      
        menuContent.SetActive(false);
    }
}

enum LanguageType
{
    korean,
    english,
}
