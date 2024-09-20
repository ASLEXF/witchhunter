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

    //private void Awake()
    //{
    //    instance = this;
    //}

    //private void Start()
    //{
    //    initializeUIs();

    //    if (DebugMode.IsDebugMode)
    //    {
    //        Show();
    //    }

    //    gameObject.SetActive(false);
    //}

    //private void initializeUIs()
    //{
    //    ItemUIs = new ItemUI[maxNumber];
    //    for (int i = 0; i < maxNumber; i++)
    //    {
    //        ItemUIs[i] = transform.GetChild(i).GetComponent<ItemUI>();
    //    }
    //}

    //public bool AddItem(Item item)
    //{
    //    bool result = false;
    //    for (int i = 0; i < maxNumber; i++)
    //    {
    //        if ()
    //    }
    //    if (!IsFull)
    //    {
    //        items.Add(item);
    //        GameEvents.Instance.ItemsUpdated();
    //        return true;
    //    }
    //    return false;
    //}

    //private void updateItems()
    //{
    //    for (int i = 0; i < items.Count; i++)
    //    {
    //        Image image = ItemUIs[i].GetComponent<Image>();
    //        Text amount = ItemUIs[i].transform.Find("Amount").GetComponent<Text>();
    //        if (items[i] is ComsumableItem)
    //        {
    //            ComsumableItem comsumableItem = items[i] as ComsumableItem;
    //            if (comsumableItem.amount == 0)
    //            {
    //                image.sprite = null;
    //                amount.text = "";
    //                items.RemoveAt(i);
    //                i--;
    //            }
    //            else
    //            {
    //                spriteAddresses.Add(comsumableItem.icon);
    //                amount.text = comsumableItem.amount.ToString();
    //            }
    //        }
    //        else if (items[i] is MaterialItem)
    //        {
    //            MaterialItem materialItem = items[i] as MaterialItem;
    //            if (materialItem.amount == 0)
    //            {
    //                image.sprite = null;
    //                amount.text = "";
    //                items.RemoveAt(i);
    //                i--;
    //            }
    //            else
    //            {
    //                SpriteRenderer itemSprite = itemObj.GetComponent<SpriteRenderer>();
    //                image.sprite = itemSprite.sprite;
    //                amount.text = materialItem.amount.ToString();
    //            }
    //        }

    //        else
    //        {
    //            image.sprite = null;
    //            amount.text = "";
    //        }
    //    }
    //}

    //public void Show()
    //{
    //    updateItems();
    //    gameObject.SetActive(true);
    //}

    //public void Hide()
    //{
    //    gameObject.SetActive(false);
    //}

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        initializeItems();

        ShowItemsUI();

        //HideItemsUI();

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
