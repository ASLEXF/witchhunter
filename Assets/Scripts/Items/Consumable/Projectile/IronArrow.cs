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
    private BoxCollider2D _collider;

    public bool isShooting { get; private set; } = false;

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

    //damage
    public int damage = 1;

    // random deviation
    [SerializeField] private float heightDeviation = 1.0f;
    [SerializeField] private float guidingStrength = 0.5f;  // how much the arrow will be guided towards the target
    [SerializeField] private float rotationDeviation = 0.75f;  // maximum random rotation deviation in degrees

    // randomly consumed chance
    [SerializeField] private float consumedChance = 0.5f;

    private void Awake()
    {
        item = new ProjectileItem(
            13, 
            "iron arrow", 
            "", 
            "Icons/iron_arrow.png", 
            "Prefabs/Items/iron_arrow.prefab",
            1
        );
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }

    private void FixedUpdate()
    {
        if (!_collider.isActiveAndEnabled || !isShooting) return;

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
        stickOnto(collision.gameObject);
    }

    public Item GetItem() => item;

    public void Hide()
    {
        spriteRenderer.enabled = false;
        _collider.enabled = false;
    }
    
    public void Show()
    {
        spriteRenderer.enabled = true;
        _collider.enabled = false;
    }

    public void UpdatePosition(Vector2 direction, bool reset = false)
    {
        spriteRenderer.sortingOrder = 1;
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

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void Shoot(Vector2 force, float heightFromGround)
    {
        if (force == null)
        {
            force = new Vector2(10, 0);
        }
        // Set the parent to environment to avoid being affected by player's movement
        transform.SetParent(Environment.Instance.gameObject.transform);
        height = heightFromGround;
        // Enable component
        gameObject.GetComponent<DroppedItem>().enabled = true;
        // Enable physics and collider
        spriteRenderer.enabled = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.gravityScale = 0.6f;
        _collider.enabled = true;
        _collider.isTrigger = false;
        rb.AddForce(force, ForceMode2D.Impulse);
        isShooting = true;
        // Add another arrow if the player has more in the inventory
        PlayerHand.Instance.Projectile = null;
        ItemBar.Instance.UpdateProjectile(item.id);
    }

    private void stickOnto(GameObject obj)
    {
        isShooting = false;
        // Set the parent to follow
        if (obj != null)
            transform.SetParent(obj.transform);
        else
            transform.SetParent(Environment.Instance.gameObject.transform);
        rb.isKinematic = true;
        // Set sorting order to be behind entities
        spriteRenderer.sortingOrder = 0;
        // Disable collider and physics
        _collider.enabled = false;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePosition;

        // Randomly rotate a bit, if it's not sticking onto ground
        if (obj != null)
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(-rotationDeviation, rotationDeviation));

        // Deal damage
        if (obj != null && obj.CompareTag("Enemy"))
        {
            NPCHealth enemyHealth = obj.GetComponentInChildren<NPCHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

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
