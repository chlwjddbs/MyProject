using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Localization.Components;

public class SkillSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SkillItem skillItem;
    public Image skillImage;
    public LocalizeStringEvent skillName;

    public void AddSkill(SkillItem _skillItem)
    {
        skillItem = _skillItem;
        skillImage.sprite = skillItem.itemImege;
        skillName.StringReference.TableEntryReference = skillItem.skillName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        skillImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        skillImage.transform.position = eventData.position;
    }

    //스킬북에 등록된 스킬을 스킬 버튼에 장착한다.
    public void OnEndDrag(PointerEventData eventData)
    {
        List<GameObject> hoveredObject = eventData.hovered;

        for (int i = 0; i < hoveredObject.Count; i++)
        {
            //드래그 한 곳이 스킬 버튼일 때
            if (hoveredObject[i].CompareTag("SkillButton"))
            {
                if (hoveredObject[i].TryGetComponent(out SkillButton skillButton))
                {
                    //스킬 버튼을 사용 가능한 상태라면
                    if (skillButton.isUse)
                    {
                        //현재 슬롯에 있는 스킬을 드래그 한 버튼에 장착
                        //장착한 스킬은 SkillBook에서 관리
                        SkillBook.instance.isEquip(skillItem, skillButton.buttonNum);
                    }
                    else
                    {
                        SequenceText.instance.SetSequenceText(null, "스킬장착불가");
                    }
                }
            }
        }
        skillImage.raycastTarget = true;
        skillImage.transform.localPosition = Vector3.zero;

        hoveredObject.Clear();
    }
}
