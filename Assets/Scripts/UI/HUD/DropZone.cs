using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DraggableItem draggableItem = eventData.pointerDrag.GetComponent<DraggableItem>();
            if (draggableItem != null)
            {
                draggableItem.transform.SetParent(transform);
                RectTransform rectTransform = draggableItem.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = Vector2.zero; // 可根据需求决定放置位置
            }
        }
    }
}
