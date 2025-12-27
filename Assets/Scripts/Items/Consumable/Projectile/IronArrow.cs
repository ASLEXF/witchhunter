using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class IronArrow : MonoBehaviour, IItem, IProjectile
{
    public Item item;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private BoxCollider2D collider;

    private int posIndex = -1;
    private Vector3[] positions = new Vector3[] {
        new Vector3(0.227f, 0.578f, 0),
        new Vector3(0.268f, 0.71f, 0),
        new Vector3(0.191f, 0.81f, 0)
    };
    private Vector3[] rotations = new Vector3[]
    {
        new Vector3(0, 0, 39.888f),
        new Vector3(0, 0, 2.385f),
        new Vector3(0, 0, 0)
    };
    private float _floorHeight = 0;

    private void Awake()
    {
        item = new Item(13, "iron arrow", "", "Assets/Addressables/Icons/iron_arrow.png", false, true, 2);
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        spriteRenderer.enabled = false;
        rb.gravityScale = 0;
        collider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (!collider.isActiveAndEnabled) return;

        if (transform.position.y == _floorHeight)
        {
            collider.enabled = false;
            rb.gravityScale = 0;
            return;
        }
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enermy"))
        {
            stickOnto(collision.gameObject);
        }
    }

    public Item GetItem() => item;

    public void UpdatePosition()
    {
        if (posIndex == -1)
        {
            spriteRenderer.enabled = true;
            posIndex++;
        }

        if (posIndex < 2)
        {
            transform.position = positions[posIndex];
            transform.rotation = Quaternion.Euler(rotations[posIndex]);
            posIndex++;
        }
        else
        {
            // interupted
            posIndex = -1;
        }
    }

    public void Shoot(Vector2 force, float floorHeight)
    {
        if (force == null)
        {
            force = new Vector2(1, 0);
        }
        _floorHeight = floorHeight;
        spriteRenderer.enabled = true; // debug
        rb.constraints = RigidbodyConstraints2D.None;
        rb.gravityScale = 0.6f;
        collider.enabled = true;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void stickOnto(GameObject obj)
    {
        transform.SetParent(obj.transform);
        collider.enabled = false;
        rb.gravityScale = 0;
    }
}
