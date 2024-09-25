using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Backpack : MonoBehaviour, IDropHandler
{
    private static Backpack instance;

    public static Backpack Instance
    {
        get { return instance; }
    }

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

    [SerializeField] ItemUI[] itemUIs;
    [SerializeField] int maxNumber = 4;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        initializeItems();

        if (DebugMode.IsDebugMode)
        {
            ShowItemsUI();
        }
        else
        {
            HideItemsUI();
        }

        GameEvents.Instance.OnItemsUpdated += updateItemsUI;
    }

    private void initializeItems()
    {
        itemUIs = new ItemUI[maxNumber];
        for (int i = 0; i < maxNumber; i++)
        {
            itemUIs[i] = transform.GetChild(i).GetComponent<ItemUI>();
        }

        gameObject.SetActive(false);
    }

    private void updateItemsUI()
    {
        for (int i = 0; i < maxNumber; i++)
        {
            itemUIs[i].UpdateUI();
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

    public Item FindItem(int itemID)
    {
        for (int i = 0; i < maxNumber; i++)
        {
            if (itemUIs[i].Item != null && itemUIs[i].Item.id == itemID)
                return itemUIs[i].Item;
        }

        return null;
    }

    public void ShowItemsUI()
    {
        updateItemsUI();
        gameObject.SetActive(true);
    }

    public void HideItemsUI()
    {
        gameObject.SetActive(false);
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

    private void OnDestroy()
    {
        GameEvents.Instance.OnItemsUpdated -= updateItemsUI;
    }
}
