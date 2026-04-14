#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class PlayerInventory : Singleton<PlayerInventory>
{
    public bool IsFull
    {
        get { return Backpack.Instance.IsFull && ItemBar.Instance.IsFull; }
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

        if (newItem is ConsumableItem)
        {
            ConsumableItem newComsumableItem = (ConsumableItem)newItem;  // cast to avoid nullable check
            ItemUI? itemUI = FindItemUI(newComsumableItem.id);
            if (itemUI != null)
            {
                ConsumableItem old = (ConsumableItem)itemUI.Item;
                old.amount += newComsumableItem.amount;
                itemUI.UpdateAmount(old.amount);
            }
            else if (!ItemBar.Instance.IsFull)
            {
                ItemBar.Instance.AddItem(newComsumableItem.DeepCopy());
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
            ItemUI? itemUI = FindItemUI(newItem.id);
            if (itemUI != null)
            {
                MaterialItem old = (MaterialItem)itemUI.Item;
                old.amount += newMaterialItem.amount;
                itemUI.UpdateAmount(old.amount);
            }
            else if (!ItemBar.Instance.IsFull)
            {
                ItemBar.Instance.AddItem(newMaterialItem.DeepCopy());
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
        else if (newItem is ProjectileItem)
        {
            ProjectileItem newProjectileItem = (ProjectileItem)newItem;
            ItemUI? itemUI = FindItemUI(newItem.id);
            if (itemUI != null)
            {
                ProjectileItem old = (ProjectileItem)itemUI.Item;
                old.amount += newProjectileItem.amount;
                itemUI.UpdateAmount(old.amount);
            }
            else if (!ItemBar.Instance.IsFull)
            {
                ItemBar.Instance.AddItem(newProjectileItem.DeepCopy());
            }
            else if (!IsFull)
            {
                Backpack.Instance.AddItem(newProjectileItem.DeepCopy());
            }
            else
            {
                HandleInventoryFull();
            }
        }
        else if (newItem is WeaponItem)
        {
            WeaponItem newWeaponItem = (WeaponItem)newItem;
            if (!ItemBar.Instance.IsFull)
            {
                ItemBar.Instance.AddItem(newWeaponItem.DeepCopy());
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
            if (!ItemBar.Instance.IsFull)
            {
                ItemBar.Instance.AddItem(newImportantItem.DeepCopy());
            }
            else if (!Backpack.Instance.IsFull)
            {
                Backpack.Instance.AddItem(newImportantItem.DeepCopy());
            }
            else
            {
                HandleInventoryFull();
            }
        }
        else
        {
            if (!ItemBar.Instance.IsFull)
            {
                ItemBar.Instance.AddItem(newItem.DeepCopy());
            }
            else if (!Backpack.Instance.IsFull)
            {
                Backpack.Instance.AddItem(newItem.DeepCopy());
            }
            else
            {
                HandleInventoryFull();
            }
        }

        GameEvents.Instance.ItemsUpdated();
    }

    public ItemUI? FindItemUI(int itemID)
    {
        ItemUI? itemUI = ItemBar.Instance.FindItemUI(itemID);
        if (itemUI != null)
        {
            return itemUI;
        }

        itemUI = Backpack.Instance.FindItemUI(itemID);
        if (itemUI != null)
        {
            return itemUI;
        }
        
        return null;
    }

    public Item? FindItem(int itemID, bool inQuickItem = false)
    {
        if (inQuickItem)
        {
            ItemUI? itemUI = ItemBar.Instance.FindItemUI(itemID);
            if (itemUI != null)
            {
                return itemUI.Item;
            }
        }
        else
        {
            ItemUI? itemUI = FindItemUI(itemID);
            if (itemUI != null)
            {
                return itemUI.Item;
            }
        }

        return null;
    }

    private void HandleInventoryFull()
    {
        Debug.Log("Player Inventory full!");
    }
}
