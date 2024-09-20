using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemsUI : MonoBehaviour
{
    private static ItemsUI instance;

    public static ItemsUI Instance
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
    [SerializeField] int maxNumber = 7;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        initializeItems();

        HideItemsUI();

        GameEvents.Instance.OnBattleModeStarted += ShowItemsUI;
        GameEvents.Instance.OnExplorationModeStarted += ShowItemsUI;
        GameEvents.Instance.OnStoryModeStarted += HideItemsUI;

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
                result =  true;
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

    private void OnDestroy()
    {
        GameEvents.Instance.OnBattleModeStarted -= ShowItemsUI;
        GameEvents.Instance.OnExplorationModeStarted -= ShowItemsUI;
        GameEvents.Instance.OnStoryModeStarted -= HideItemsUI;

        GameEvents.Instance.OnItemsUpdated -= updateItemsUI;
    }
}
