using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class ItemDrop : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    [SerializeField] bool isDrop = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Interacted()
    {
        if (isDrop)
        {
            if (!PlayerInventory.Instance.IsFull || !ItemsUI.Instance.IsFull)
            {
                PlayerInventory.Instance.AddItem(gameObject.GetComponent<IItem>().GetItem());
                Destroy(gameObject);
                //isDrop = false;
                //spriteRenderer.enabled = false;
                //boxCollider.enabled = false;
            }
        }
    }
}
