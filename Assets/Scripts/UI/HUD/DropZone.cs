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
                ItemUI itemUI = draggableItem.GetComponent<ItemUI>();

                if (Backpack.Instance.AddItem(itemUI.Item))
                {
                    itemUI.Item = null;
                }

                //RectTransform rectTransform = draggableItem.GetComponent<RectTransform>();
                //rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }
}
