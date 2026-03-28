using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class DroppedItem : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    [SerializeField] bool interactable = true;
    [SerializeField] public Vector3 UIOffset = new Vector3(0f, 0.5f, 0f);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        gameObject.tag = "DropItem";
    }

    private void Start()
    {
        interactable = true;
    }

    public void Interacted()
    {
        if (interactable)
        {
            if (!PlayerInventory.Instance.IsFull)
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
