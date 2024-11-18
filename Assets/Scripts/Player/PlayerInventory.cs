/* sample */
# nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class PlayerInventory : MonoBehaviour
{
    // use singleton here for single player game
    private static PlayerInventory instance;
    public static PlayerInventory Instance
    {
        get { return instance; }
    }

    public bool IsFull
    {
        get { return Backpack.Instance.IsFull && ItemsUI.Instance.IsFull; }
    }

    private void Awake()
    {
        instance = this;
    }

    public async Task AddItem(int id, int count = 1)
    {
        Item item = await ItemManager.Instance.GenerateItem(id, count);
        AddItem(item);
    }

    public void AddItem(Item? newItem)
    {
        if (newItem is null) 
        {
            Debug.LogError("Wrong game object for AddItem func!");
            throw new ArgumentNullException(nameof(newItem), "Item cannot be null.");
        }

        if (newItem is ComsumableItem)
        {
            ComsumableItem newComsumableItem = (ComsumableItem)newItem;  // cast to avoid nullable check
            Item? oldItem = FindItem(newComsumableItem.id);
            if (oldItem != null)
            {
                ComsumableItem oldComsumableItem = (ComsumableItem)oldItem;
                oldComsumableItem.amount += newComsumableItem.amount;
            }
            else if (!ItemsUI.Instance.IsFull)
            {
                ItemsUI.Instance.AddItem(newComsumableItem.DeepCopy());
            }
            else if (!IsFull)
            {
                Backpack.Instance.AddItem(newComsumableItem.DeepCopy());
            }
            else
            {
                HandleInventoryFull();
            }
        }
        else if (newItem is MaterialItem)
        {
            MaterialItem newMaterialItem = (MaterialItem)newItem;
            Item? oldItem = FindItem(newItem.id);
            if (oldItem != null)
            {
                MaterialItem oldComsumableItem = (MaterialItem)oldItem;
                oldComsumableItem.amount += newMaterialItem.amount;
            }
            else if (!ItemsUI.Instance.IsFull)
            {
                ItemsUI.Instance.AddItem(newMaterialItem.DeepCopy());
            }
            else if (!IsFull)
            {
                Backpack.Instance.AddItem(newMaterialItem.DeepCopy());
            }
            else
            {
                HandleInventoryFull();
            }
        }
        else if (newItem is WeaponItem)
        {
            WeaponItem newWeaponItem = (WeaponItem)newItem;
            if (!ItemsUI.Instance.IsFull)
            {
                ItemsUI.Instance.AddItem(newWeaponItem.DeepCopy());
            }
            else if (!IsFull)
            {
                Backpack.Instance.AddItem(newWeaponItem.DeepCopy());
            }
            else
            {
                HandleInventoryFull();
            }
        }
        else if (newItem is ImportantItem)
        {
            ImportantItem newImportantItem = (ImportantItem)newItem;
            if (!IsFull)
            {
                Backpack.Instance.AddItem(newImportantItem.DeepCopy());
            }
            else if (!ItemsUI.Instance.IsFull)
            {
                ItemsUI.Instance.AddItem(newImportantItem.DeepCopy());
            }
            else
            {
                HandleInventoryFull();
            }
        }
        else
        {
            if (!IsFull)
            {
                Backpack.Instance.AddItem(newItem.DeepCopy());
            }
            else if (!ItemsUI.Instance.IsFull)
            {
                ItemsUI.Instance.AddItem(newItem.DeepCopy());
            }
            else
            {
                HandleInventoryFull();
            }
        }

        GameEvents.Instance.ItemsUpdated();
    }

    public Item? FindItem(int itemID)
    {
        Item itemUI = ItemsUI.Instance.FindItem(itemID);
        if (itemUI != null)
        {
            return itemUI;
        }

        itemUI = Backpack.Instance.FindItem(itemID);
        if (itemUI != null)
        {
            return itemUI;
        }
        
        return null;
    }

    private void HandleInventoryFull()
    {
        Debug.Log("Player Inventory full!");
    }
}
