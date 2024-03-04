using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBookUI : MonoBehaviour
{
    public GameObject skillBookUI;
    private RectTransform skillBookUIrect;
    private bool isOpen;

    public Sound[] skillboolSound;

    private void Start()
    {
        foreach (var s in skillboolSound)
        {
            AudioManager.instance.AddExternalSound(s);
        }
    }

    public void SetData()
    {
        skillBookUIrect = skillBookUI.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!DataManager.instance.isSet)
        {
            return;
        }

        if (GameSceneMenu.isMenuOpen)
        {
            return;
        }

        ToggleUI();
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            skillBookUI.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            skillBookUI.SetActive(!skillBookUI.activeSelf);
        }
        */
    }

    public void ToggleUI()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen)
            {
                CloseUI();
                AudioManager.instance.PlayExSound("closeSkillbook");
            }
        }
        

        if (Input.GetKeyDown(KeyCode.K))
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                skillBookUIrect.anchoredPosition = new Vector3(0, 0, 0);
                AudioManager.instance.PlayExSound("openSkillbook");
            }
            else
            {
                skillBookUIrect.anchoredPosition = new Vector3(-1920f, 0, 0);
                AudioManager.instance.PlayExSound("closeSkillbook");
            }
        }
    }

    public void CloseUI()
    {
        isOpen = false;
        skillBookUIrect.anchoredPosition = new Vector3(-1920f, 0, 0);
    }

    public void OpenUI()
    {
        isOpen = true;
        skillBookUIrect.anchoredPosition = new Vector3(0, 0, 0);
    }

    public bool UIOpenCheck()
    {
        return isOpen;
    }
}
