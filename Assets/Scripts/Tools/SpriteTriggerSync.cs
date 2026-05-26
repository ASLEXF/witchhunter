using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class SpriteTriggerSync : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    PolygonCollider2D polygonCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        polygonCollider.isTrigger = true;
        UpdateCollider();
    }

    private void Update()
    {
        UpdateCollider();
    }

    private void UpdateCollider()
    {
        if (spriteRenderer != null)
        {
            Sprite sprite = spriteRenderer.sprite;
            List<Vector2> path = new List<Vector2>();
            for (int i = 0; i < polygonCollider.pathCount; i++)
            {
                path.Clear();
                sprite.GetPhysicsShape(i, path);
                polygonCollider.SetPath(i, path.ToArray());
            }
        }
        else
        {
            Debug.LogWarning($"Sprite renderer on {transform.parent.name} not found!");
        }
    }
}
