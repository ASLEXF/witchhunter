using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Backpack : Singleton<Backpack>, IDropHandler
{
    public int ItemNumber
    {
        get
        {
            int result = 0;
            for (int i = 0; i < maxNumber; i++)
            {
                if (!itemUIs[i].Empty) result++;
            }

            return result;
        }
    }

    public bool IsFull
    {
        get
        {
            return ItemNumber == maxNumber;
        }
    }

    public bool IsEmpty
    {
        get
        {
            return ItemNumber == 0;
        }
    }

    Image image;

    [SerializeField] ItemUI[] itemUIs;
    [SerializeField] int maxNumber = 4;

    protected override void Awake()
    {
        base.Awake();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        initializeItems();
    }

    private void OnEnable()
    {
        GameEvents.Instance.OnItemsUpdated += num => updateItemsUI(num);
    }

    private void OnDestroy()
    {
        if (GameEvents.HasInstance)
        { GameEvents.Instance.OnItemsUpdated -= num => updateItemsUI(num); }
    }

    private void initializeItems()
    {
        itemUIs = new ItemUI[maxNumber];
        for (int i = 0; i < maxNumber; i++)
        {
            itemUIs[i] = transform.GetChild(i).GetComponent<ItemUI>();
        }

        if (IsEmpty)
        {
            Hide();
        }
    }

    private void updateItemsUI(int hint = -1)
    {
        if (hint == -1)
        {
            for (int i = 0; i < maxNumber; i++)
            {
                itemUIs[i].UpdateUI();
            }
        }
        else if (hint >= 0 && hint < maxNumber)
        {
            itemUIs[hint].UpdateUI();
        }
    }

    public bool AddItem(Item newItem)
    {
        bool result = false;
        for (int i = 0; i < maxNumber; i++)
        {
            if (itemUIs[i].Empty)
            {
                itemUIs[i].Item = newItem;
                result = true;
                break;
            }
        }
        if (result)
            GameEvents.Instance.ItemsUpdated();
        return result;
    }

    public ItemUI FindItemUI(int itemID)
    {
        for (int i = 0; i < maxNumber; i++)
        {
            if (itemUIs[i].Item != null && itemUIs[i].Item.id == itemID)
                return itemUIs[i];
        }

        return null;
    }

    public void ShowOrHide()
    {
        if (image.enabled)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        updateItemsUI();
        image.enabled = true;
        foreach (ItemUI itemUI in itemUIs)
        {
            itemUI.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        image.enabled = false;
        foreach (ItemUI itemUI in itemUIs)
        {
            itemUI.gameObject.SetActive(false);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DraggableItem draggableItem = eventData.pointerDrag.GetComponent<DraggableItem>();
            if (draggableItem != null)
            {
                ItemUI itemUI = draggableItem.GetComponent<ItemUI>();

                if (AddItem(itemUI.Item))
                {
                    itemUI.Item = null;
                }
            }
        }
    }
}
