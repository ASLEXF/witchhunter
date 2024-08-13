using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance;

    public static PlayerController Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("PlayerController");
                _instance = singletonObject.AddComponent<PlayerController>();
            }
            return _instance;
        }
    }

    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Animator _animator;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        _animator.SetBool(Animator.StringToHash("IsMoving"), Mathf.Abs(horizontalInput) > 0f || Mathf.Abs(verticalInput) > 0f);

        //// debug
        //Vector3 startPos = transform.position;
        //Vector3 endPos = transform.position + new Vector3(100, 100, 100);
        //Color lineColor = Color.red;
        //float duration = 5.0f;
        //Debug.DrawLine(startPos, endPos, lineColor, duration);
    }

    private void FixedUpdate()
    {
        //Vector2 moveDirection = new Vector2(horizentalInput, verticalInput);
        //rb.velocity = moveDirection * movementSpeed;
        if (canMove)
        {
            TurnThePlayer();
            MoveThePlayer();
        }
    }

    #region Display

    public void HideThePlayer()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        Rigidbody2D[] rigidbodies = GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D rb in rigidbodies)
        {
            rb.simulated = false;
        }

        enabled = false;
    }

    public void ShowThePlayer()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = true;
        }

        Rigidbody2D[] rigidbodies = GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D rb in rigidbodies)
        {
            rb.simulated = true;
        }
    }

    #endregion

    #region Movement
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float movementSmoothingSpeed = 1.0f;
    public bool canMove = true;
    Vector3 _rawInputMovement;

    void TurnThePlayer()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput > 0f)
        {
            _spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0f)
        {
            _spriteRenderer.flipX = true;
        }
    }

    void MoveThePlayer()
    {
        Vector3 movement = _rawInputMovement * movementSpeed * Time.deltaTime;
        _rb.MovePosition(transform.position + movement);
    }

    #endregion

    #region Input System

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        _rawInputMovement = new Vector3(inputMovement.x, inputMovement.y, 0);
        //smoothInputMovement = Vector3.Lerp(smoothInputMovement, rawInputMovement, Time.deltaTime * movementSmoothingSpeed);
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            canMove = false;
            _animator.SetTrigger(Animator.StringToHash("Attack"));
        }
    }

    public void OnSupport(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            canMove = false;
            _animator.SetTrigger(Animator.StringToHash("Support"));
        }
    }

    public void OnInteract(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            
        }
    }

    #endregion
}
