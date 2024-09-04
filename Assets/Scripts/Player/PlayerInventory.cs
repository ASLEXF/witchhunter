using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] int capacity = 10;
    [SerializeField] List<GameObject> items;

    private void Start()
    {
        items = new List<GameObject>();
    }

    public void AddItem(GameObject item)
    {
        if (items.Count == capacity) return;
        items.Add(item);
    }

    public void RemoveItem(GameObject item)
    {
        items.Remove(item);
    }
}
