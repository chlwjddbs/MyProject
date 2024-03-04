using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    public ScrollRect ParentView;

    // Start is called before the first frame update
    public void OnBeginDrag(PointerEventData eventData)
    {
        ParentView.OnBeginDrag(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        ParentView.OnDrag(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        ParentView.OnEndDrag(eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
        ParentView.OnScroll(eventData);
        Debug.Log("Check");
    }
}
