using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnMouseOverUI : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler
{
    private Image button;
    private Color orignColor = new Color(255, 255, 255);
    private Color mouseoverColor = new Color(0, 255, 128);
  
    private void Awake()
    {
        button = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        button.color = mouseoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        button.color = orignColor;
    }

    private void OnDisable()
    {
        button.color = orignColor;
    }

}
