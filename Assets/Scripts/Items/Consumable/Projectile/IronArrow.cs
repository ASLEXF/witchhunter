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

    // arrow position for animation
    private int posIndex = 0;
    private Vector3[] positions = new Vector3[] {
        new Vector3(0.352f, 0.656f, 0),  // bow_1
        new Vector3(0.341f, 0.682f, 0),  // bow_3
        new Vector3(0.216f, 0.786f, 0)  // bow_4
    };
    private Vector3[] rotations = new Vector3[]
    {
        new Vector3(0, 0, 39.888f),
        new Vector3(0, 0, 2.385f),
        new Vector3(0, 0, 0)
    };

    // floor height for checking if the arrow should stick onto the ground
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
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        collider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (!collider.isActiveAndEnabled) return;

        if (transform.position.y <= _floorHeight)
        {
            stickOnto(null);
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

    public void UpdatePosition(bool reset = false)
    {
        if (reset)
        {
            transform.localPosition = positions[0];
            transform.localRotation = Quaternion.Euler(rotations[0]);
            spriteRenderer.enabled = true;
            posIndex = 1;
            return;
        }
        
        if (posIndex < 3)
        {
            transform.localPosition = positions[posIndex];
            transform.localRotation = Quaternion.Euler(rotations[posIndex]);
            posIndex++;
        }
        else
        {
            // interupted
            posIndex = 0;
        }
    }

    public void Shoot(Vector2 force, float floorHeight)
    {
        if (force == null)
        {
            force = new Vector2(1, 0);
        }

        transform.SetParent(Environment.Instance.gameObject.transform);
        _floorHeight = floorHeight;

        spriteRenderer.enabled = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.gravityScale = 0.6f;
        collider.enabled = true;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void stickOnto(GameObject obj)
    {
        // Set the parent to follow
        if (obj != null)
            transform.SetParent(obj.transform);
        else
            transform.SetParent(Environment.Instance.gameObject.transform);
        rb.isKinematic = true;

        // Disable collider and physics
        collider.enabled = false;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        // Randomly rotate a bit, if it's not sticking onto ground
        if (obj != null)
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(-.75f, .75f));

        // TODO: damage calculation
        // TODO: Randomly Consumed
        // TODO: destroy after some time
    }
}
