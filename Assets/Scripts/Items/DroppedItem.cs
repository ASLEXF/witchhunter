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
        gameObject.tag = "DroppedItem";
    }

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Item");
    }

    private void OnEnable()
    {
        interactable = true;
        boxCollider.enabled = true;
        boxCollider.isTrigger = true;
    }

    private void OnDisable()
    {
        interactable = false;
        boxCollider.enabled = false;
        boxCollider.isTrigger = false;
    }

    public void Interacted()
    {
        if (!interactable || PlayerInventory.Instance.IsFull) return;

        var item = GetComponentInParent<IItem>();
        if (item == null)
            return;
        PlayerInventory.Instance.AddItem(item.GetItem());
        Destroy(transform.parent.gameObject);
    }
}
