using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rope : MonoBehaviour, IItem
{
    public MaterialItem item;

    private void Awake()
    {
        item = new MaterialItem(8, "Rope", "a rope, can be used to make something", "Icons/rope.png", 1);
    }

    public Item GetItem() => item;

    public void Hide() => gameObject.SetActive(false);

    public void Show() => gameObject.SetActive(true);
}
