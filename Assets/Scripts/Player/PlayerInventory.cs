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

    public bool IsFull
    {
        get { return Backpack.Instance.IsFull && ItemsUI.Instance.IsFull; }
    }

    private void Awake()
    {
        instance = this;
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
                Debug.Log("Player Inventory full!");
            }
        }
        else if (newItem is WeaponItem)
        {
            WeaponItem newWeaponItem = newItem as WeaponItem;
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
                Debug.Log("Player Inventory full!");
            }
        }
        else if (newItem is ImportantItem)
        {
            ImportantItem newImportantItem = newItem as ImportantItem;
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
                Debug.Log("Player Inventory full!");
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

        itemUI = Backpack.Instance.FindItem(itemID);
        if (itemUI != null)
        {
            return itemUI;
        }
        
        return null;
    }
}
