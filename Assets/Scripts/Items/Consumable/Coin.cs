using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Coin : MonoBehaviour, IItem
{
    public MaterialItem item;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        item = new MaterialItem(0, "Coin", "Money", "", 1);
    }

    public Item GetItem()
    {
        return item;
    }
}
