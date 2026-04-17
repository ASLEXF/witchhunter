# nullable enable

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
    }

    private void OnEnable()
    {
        GameEvents.Instance.OnBattleModeStarted += Show;
        GameEvents.Instance.OnExplorationModeStarted += Show;
        GameEvents.Instance.OnStoryModeStarted += Hide;
        GameEvents.Instance.OnItemsUpdated += num => updateItemsUI(num);
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnBattleModeStarted -= Show;
        GameEvents.Instance.OnExplorationModeStarted -= Show;
        GameEvents.Instance.OnStoryModeStarted -= Hide;
        GameEvents.Instance.OnItemsUpdated -= num => updateItemsUI(num);
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

    public void AddItem(Item newItem)
    {
        for (int i = 0; i < maxNumber; i++)
        {
            if (itemUIs[i].Empty)
            {
                itemUIs[i].Item = newItem;
                GameEvents.Instance.ItemsUpdated(i);
                break;
            }
        }

        Show();
    }

    public ItemUI? FindItemUI(int itemID)
    {
        for (int i = 0; i < maxNumber; i++)
        {
            if (itemUIs[i].Item != null && itemUIs[i].Item.id == itemID) 
                return itemUIs[i];
        }

        return null;
    }

    public void UpdateProjectile(int itemID, int number = 1)
    {
        for (int i = 0; i < maxNumber; i++)
        {
            Item item = itemUIs[i].Item;
            if (item != null && item.id == itemID)
            {
                if (item is ProjectileItem projectileItem)
                {
                    projectileItem.amount -= number;
                    GameEvents.Instance.ItemsUpdated(i);
                    if (projectileItem.amount > 0)
                    {
                        PlayerHand.Instance.EquipProjectile(projectileItem);
                    }
                }
                else
                {
                    Debug.LogError("Unknown item type for function ItemAmountMinus()");
                }
            }
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
}
