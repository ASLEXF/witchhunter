using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class BoneArrow : MonoBehaviour, IItem, IProjectile
{
    public Item item;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private BoxCollider2D _collider;

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
    private float height = 0;

    // damage
    public int damage = 1;

    // random deviation
    [SerializeField] private float heightDeviation = 1.0f;
    [SerializeField] private float guidingStrength = 0.3f;  // how much the arrow will be guided towards the target
    [SerializeField] private float rotationDeviation = 0.75f;  // maximum random rotation deviation in degrees

    // randomly consumed chance
    [SerializeField] private float consumedChance = 0.8f;

    private void Awake()
    {
        item = new ProjectileItem(
            15, 
            "bone arrow", 
            "", 
            "Icons/bone_arrow.png", 
            "Prefabs/Items/bone_arrow.prefab",
            1
        );
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        spriteRenderer.enabled = false;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        _collider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (!_collider.isActiveAndEnabled) return;

        if (transform.position.y <= height)
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

    public void Hide() => gameObject.SetActive(false);

    public void Show() => gameObject.SetActive(true);

    public void UpdatePosition(Vector2 direction, bool reset = false)
    {
        spriteRenderer.flipX = direction.x < 0;

        if (reset)
        {
            transform.localPosition = new Vector3(
                positions[0].x * direction.x,
                positions[0].y,
                positions[0].z
            );
            transform.localRotation = Quaternion.Euler(rotations[0]);
            spriteRenderer.enabled = true;
            posIndex = 1;
            return;
        }

        if (posIndex < 3)
        {
            transform.localPosition = new Vector3(
                positions[posIndex].x * direction.x,
                positions[posIndex].y,
                positions[posIndex].z
            );
            transform.localRotation = Quaternion.Euler(rotations[posIndex]);
            posIndex++;
        }
        else
        {
            // interupted
            posIndex = 0;
        }
    }

    public void Shoot(Vector2 force, float heightFromGround)
    {
        if (force == null)
        {
            force = new Vector2(0, 1);
        }

        transform.SetParent(Environment.Instance.gameObject.transform);
        height = heightFromGround;

        spriteRenderer.enabled = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.gravityScale = 0.6f;
        _collider.enabled = true;
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
        _collider.enabled = false;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        // Randomly rotate a bit, if it's not sticking onto ground
        if (obj != null)
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(-rotationDeviation, rotationDeviation));

        // TODO: damage calculation

        // Randomly Consumed
        float random = Random.value;
        if (random > consumedChance)
        {
            gameObject.AddComponent<DroppedItem>();
        }
        else
        {
            StartCoroutine(destroyAfterSeconds());
        }
    }

    private IEnumerator destroyAfterSeconds(float seconds = 7f)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
