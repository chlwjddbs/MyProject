using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICheck : MonoBehaviour//, IPointerExitHandler, IPointerEnterHandler
{
    private void Update()
    {
        if (!DataManager.instance.isSet)
        {
            return;
        }

        if (PlayerController.isUI && !EventSystem.current.IsPointerOverGameObject())
        {
            PlayerController.isUI = false;
        }
        else if(!PlayerController.isUI && EventSystem.current.IsPointerOverGameObject())
        {
            PlayerController.isUI = true;
        }
    }

    /*
    private void OnDisable()
    {
        PlayerController.isUI = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {         
        PlayerController.isUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerController.isUI = false;
    }
    */
    
}
