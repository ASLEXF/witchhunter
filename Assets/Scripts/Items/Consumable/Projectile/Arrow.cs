using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;

public class Arrow : MonoBehaviour, IItem, IProjectile
{
    public Item _item;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;

    private GameObject _spriteObj;
    private DroppedItem _droppedItem;

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
    [SerializeField] private float destroyDelay = 5f;

    private void Awake()
    {
        _item = new ProjectileItem(
            13, 
            "iron arrow", 
            "", 
            "Icons/iron_arrow.png", 
            "Prefabs/Items/iron_arrow.prefab",
            1
        );
        _spriteObj = transform.Find("Sprite").gameObject;
        _spriteRenderer = _spriteObj.GetComponent<SpriteRenderer>();
        _rb = _spriteObj.GetComponent<Rigidbody2D>();
        _collider = _spriteObj.GetComponent<BoxCollider2D>();
        _droppedItem = _spriteObj.GetComponent<DroppedItem>();
    }

    private void Start()
    {
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
    }

    private void FixedUpdate()
    {
        if (!_collider.isActiveAndEnabled || !isShooting) return;

        if (_spriteObj.transform.position.y <= height)
        {
            stickInto(null);
            return;
        }
        float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
        _spriteObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Hit(Collider2D other)
    {
        if (!isShooting) return;
        if (other.gameObject.CompareTag("Player")) return;  // avoid sticking onto player
        if (
            other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")
            || other.gameObject.layer == LayerMask.NameToLayer("Item")
        )
        {
            Debug.Log("Arrow hit ignored layer: " + other.gameObject.layer);
            return;
        }
                

        stickInto(other.gameObject);
    }

    public Item GetItem() => _item;

    public void Hide()
    {
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
    }
    
    public void Show()
    {
        _spriteRenderer.enabled = true;
        _collider.enabled = false;
    }

    public void UpdatePosition(Vector2 direction, bool reset = false)
    {
        _spriteRenderer.sortingOrder = 1;
        _spriteRenderer.flipX = direction.x < 0;

        if (reset)
        {
            transform.localPosition = new Vector3(
                positions[0].x * direction.x, 
                positions[0].y,
                positions[0].z
            );
            transform.localRotation = Quaternion.Euler(rotations[0]);
            _spriteRenderer.enabled = true;
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
        // reset from animation state
        _spriteRenderer.flipX = false;
        // Set the parent to environment to avoid being affected by player's movement
        transform.SetParent(Environment.Instance.Projectiles.transform);
        height = heightFromGround;
        // Disable item interaction
        _droppedItem.enabled = false;
        // Enable physics and collider
        _spriteRenderer.enabled = true;
        _rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints2D.None;
        _rb.gravityScale = 0.6f;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _collider.enabled = true;
        _collider.isTrigger = false;
        _rb.AddForce(force, ForceMode2D.Impulse);
        isShooting = true;
        // Set layer to projectile to avoid certain collisions
        gameObject.layer = LayerMask.NameToLayer("Projectile");
        // Add another arrow if the player has more in the inventory
        PlayerHand.Instance.Projectile = null;
        ItemBar.Instance.UpdateProjectile(_item.id);
    }

    private void stickInto(GameObject obj)
    {
        Debug.Log("Stick " + (obj != null ? obj.name : "ground"));
        isShooting = false;
        // Set the parent to follow
        if (obj != null)
            transform.SetParent(obj.transform);
        else
            transform.SetParent(Environment.Instance.Projectiles.transform);
        _rb.isKinematic = true;
        // Set sorting order
        _spriteRenderer.sortingOrder = 0;
        // Enable item interaction
        _droppedItem.enabled = true;
        // Disable physics
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePosition;
        // Set layer to default to interact with environment
        gameObject.layer = LayerMask.NameToLayer("Default");
        // Randomly rotate a bit, if it's not sticking onto ground
        if (obj != null)
            _spriteObj.transform.rotation = Quaternion.Euler(0, 0, _spriteObj.transform.rotation.eulerAngles.z + Random.Range(-rotationDeviation, rotationDeviation));
        // Deal damage
        if (obj != null && obj.CompareTag("Enemy"))
        {
            NPCAttacked attacked = obj.GetComponentInChildren<NPCAttacked>();
            if (attacked != null)
            {
                attacked.GetAttacked(damage, 0, _collider);
            }
        }
        // Randomly Consumed
        float random = Random.value;
        if (random < consumedChance)
            StartCoroutine(destroyAfterSeconds());
    }

    private IEnumerator destroyAfterSeconds()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
