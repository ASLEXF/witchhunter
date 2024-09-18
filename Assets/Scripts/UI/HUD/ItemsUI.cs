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
                if (!ItemUIs[i].Empty) result++;
            }

            return result;
        }
    }

    public bool isFull
    {
        get
        {
            return ItemNumber == maxNumber;
        }
    }

    [SerializeField] ItemUI[] ItemUIs;
    [SerializeField] private int maxNumber = 7;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        initializeItems();

        hideItemsUI();

        GameEvents.Instance.OnBattleModeStarted += showItemsUI;
        GameEvents.Instance.OnExplorationModeStarted += showItemsUI;
        GameEvents.Instance.OnStoryModeStarted += hideItemsUI;

        GameEvents.Instance.OnItemsUpdated += updateItemsUI;
    }

    private void initializeItems()
    {
        ItemUIs = new ItemUI[maxNumber];
        for (int i = 0; i < maxNumber; i++)
        {
            ItemUIs[i] = transform.GetChild(i).GetComponent<ItemUI>();
        }

        gameObject.SetActive(false);
    }

    private void updateItemsUI()
    {
        for (int i = 0; i < maxNumber; i++)
        {
            ItemUIs[i].UpdateUI();
        }
    }

    public void AddItem(Item newItem)
    {
        for (int i = 0; i < maxNumber; i++)
        {
            if (ItemUIs[i].Empty)
            {
                ItemUIs[i].Item = newItem;
                return;
            }
        }
        GameEvents.Instance.ItemsUpdated();
    }

    public Item FindItem(int itemID)
    {
        for (int i = 0; i < maxNumber; i++)
        {
            if (ItemUIs[i].Item != null && ItemUIs[i].Item.id == itemID) 
                return ItemUIs[i].Item;
        }

        return null;
    }

    private void showItemsUI()
    {
        updateItemsUI();
        gameObject.SetActive(true);
    }

    private void hideItemsUI()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameEvents.Instance.OnItemsUpdated -= updateItemsUI;
        GameEvents.Instance.OnBattleModeStarted -= showItemsUI;
        GameEvents.Instance.OnExplorationModeStarted -= showItemsUI;
    }
}
