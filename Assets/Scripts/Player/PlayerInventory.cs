using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private static PlayerInventory instance;
    public static PlayerInventory Instance
    {
        get { return instance; }
    }

    [SerializeField] int capacity = 10;
    [SerializeField] List<Item> items;

    public bool isFull
    {
        get { return items.Count == capacity; }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        items = new List<Item>();
    }

    public void AddItem(Item newItem)
    {
        if (newItem == null) 
        {
            Debug.LogError("Wrong game object type for AddItem func!");
        }

        if (newItem is ComsumableItem)
        {
            ComsumableItem newComsumableItem = newItem as ComsumableItem;
            Item oldItem = FindItem(newComsumableItem.id);
            if (oldItem != null)
            {
                ComsumableItem oldComsumableItem = oldItem as ComsumableItem;
                oldComsumableItem.amount += newComsumableItem.amount;
            }
            else if (!ItemsUI.Instance.isFull)
            {
                ItemsUI.Instance.AddItem(newComsumableItem.DeepCopy());
            }
            else if (!isFull)
            {
                items.Add(newComsumableItem.DeepCopy());
            }
            else
            {
                Debug.Log("Player Inventory full!");
            }
        }
        else if (newItem is MaterialItem)
        {
            MaterialItem newMaterialItem = newItem as MaterialItem;
            Item oldItem = FindItem(newItem.id);
            if (oldItem != null)
            {
                MaterialItem oldComsumableItem = oldItem as MaterialItem;
                oldComsumableItem.amount += newMaterialItem.amount;
            }
            else if (!ItemsUI.Instance.isFull)
            {
                ItemsUI.Instance.AddItem(newMaterialItem.DeepCopy());
            }
            else if (!isFull)
            {
                items.Add(newMaterialItem.DeepCopy());
            }
            else
            {
                Debug.Log("Player Inventory full!");
            }
        }
        else if (newItem is WeaponItem)
        {
            WeaponItem newWeaponItem = newItem as WeaponItem;
            if (!ItemsUI.Instance.isFull)
            {
                ItemsUI.Instance.AddItem(newWeaponItem.DeepCopy());
            }
            else if (!isFull)
            {
                items.Add(newWeaponItem.DeepCopy());
            }
            else
            {
                Debug.Log("Player Inventory full!");
            }
        }
        else if (newItem is ImportantItem)
        {
            ImportantItem newImportantItem = newItem as ImportantItem;
            if (!isFull)
            {
                items.Add(newImportantItem.DeepCopy());
            }
            else if (!ItemsUI.Instance.isFull)
            {
                ItemsUI.Instance.AddItem(newImportantItem.DeepCopy());
            }
            else
            {
                Debug.Log("Player Inventory full!");
            }
        }
        else
        {
            if (!isFull)
            {
                items.Add(newItem.DeepCopy());
            }
            else if (!ItemsUI.Instance.isFull)
            {
                ItemsUI.Instance.AddItem(newItem.DeepCopy());
            }
            else
            {
                Debug.Log("Player Inventory full!");
            }
        }

        GameEvents.Instance.ItemsUpdated();
    }

    public Item FindItem(int itemID)
    {
        Item itemUI = ItemsUI.Instance.FindItem(itemID);
        if (itemUI != null)
        {
            return itemUI;
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == itemID) return items[i];
            }
        }
        
        return null;
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }
}
