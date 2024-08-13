using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class SpriteTriggerSync : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        boxCollider.isTrigger = true;
    }

    private void Update()
    {
        if (spriteRenderer != null)
        {
            Bounds spriteBounds = spriteRenderer.bounds;
            boxCollider.size = spriteBounds.size;
            boxCollider.offset = spriteBounds.center - transform.position;
        }
        else
        {
            Debug.LogWarning("sprite render not found!");
        }
    }
}
