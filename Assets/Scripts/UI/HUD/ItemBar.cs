using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemBar : Singleton<ItemBar>
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
    [SerializeField] int maxNumber = 7;

    protected override void Awake()
    {
        base.Awake();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        initializeItems();

        GameEvents.Instance.OnBattleModeStarted += Show;
        GameEvents.Instance.OnExplorationModeStarted += Show;
        GameEvents.Instance.OnStoryModeStarted += Hide;

        GameEvents.Instance.OnItemsUpdated += updateItemsUI;
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
                result =  true;
                break;
            }
        }
        if (result)
        {
            GameEvents.Instance.ItemsUpdated();
            Show();
        }
            
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

    private void OnDestroy()
    {
        GameEvents.Instance.OnBattleModeStarted -= Show;
        GameEvents.Instance.OnExplorationModeStarted -= Show;
        GameEvents.Instance.OnStoryModeStarted -= Hide;

        GameEvents.Instance.OnItemsUpdated -= updateItemsUI;
    }
}
