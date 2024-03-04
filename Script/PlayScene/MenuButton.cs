using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MenuButton : MonoBehaviour, IPointerEnterHandler , IPointerClickHandler
{
    public string activeButton = "TouchMenu";
    public string disableButton = "DisableMenu";
    private Button bt;

    public string activeButtonSelect = "SelectMenu";
    public string disableButtonSelect;

    public Image buttonFrame;
    public TextMeshProUGUI buttonText;

    private Color orign_frameClolr;
    private Color orign_textColor;

    public Color disabled_framColor;
    public Color disabled_textColor;

    private void Awake()
    {
        SetButton();
    }

    public void SetButton()
    {
        //Debug.Log(gameObject.name);
        bt = GetComponent<Button>();

        if (buttonFrame != null)
        {
            orign_frameClolr = buttonFrame.color;

            if (bt.interactable == false)
            {
                buttonFrame.color = disabled_framColor;
            }
        }

        if (buttonText != null)
        {
            orign_textColor = buttonText.color;

            if (bt.interactable == false)
            {
                buttonText.color = disabled_textColor;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bt.interactable)
        {
            AudioManager.instance.PlayeSound(activeButton);
        }
        else
        {
            AudioManager.instance.PlayeSound(disableButton);
        }
    }

    public void ButtonClick()
    {
        AudioManager.instance.PlayeSound(activeButtonSelect);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!bt.interactable)
        {
            AudioManager.instance.PlayeSound(disableButtonSelect);
        }
    }

    public void ChangeButtonState(bool _interactable)
    {
        if (bt.interactable == _interactable)
        {
            return;
        }
        //Debug.Log(gameObject.name + "1");
        bt.interactable = _interactable;

        if (bt.interactable)
        {
            if (buttonFrame != null)
            {
                buttonFrame.color = orign_frameClolr;
            }

            if (buttonText != null)
            {
                buttonText.color = orign_textColor;
            }
        }
        else
        {
            if (buttonFrame != null)
            {
                buttonFrame.color = disabled_framColor;
            }

            if (buttonText != null)
            {
                buttonText.color = disabled_textColor;
            }
        }
    }

    
}
