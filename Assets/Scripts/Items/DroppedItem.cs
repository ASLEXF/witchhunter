using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class DroppedItem : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    [SerializeField] bool isDropped = true;
    [SerializeField] public Vector3 UIOffset = new Vector3(0f, 0.5f, 0f);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        gameObject.tag = "DropItem";
        isDropped = true;
    }

    public void Interacted()
    {
        if (isDropped)
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
