using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsUI : MonoBehaviour
{
    [SerializeField] GameObject[] items;

    private void Awake()
    {
        
    }

    private void Start()
    {
        initializeItems();
    }

    private void initializeItems()
    {
        items = new GameObject[transform.childCount];
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = transform.GetChild(i).gameObject;
        }
    }
}
