using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Timeline;

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
                Debug.LogWarning("Player Controller null");
                GameObject singletonObject = new GameObject("PlayerController");
                _instance = singletonObject.AddComponent<PlayerController>();
            }
            return _instance;
        }
    }

    Vector2 facePosition;

    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Animator _animator;
    PlayerAttack _attack;
    PlayerInteract _interact;

    void Awake()
    {
        //if (_instance != null && _instance != this)
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
        //    _instance = this;
        //}
        _instance = this;

        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _attack = transform.GetChild(0).GetComponent<PlayerAttack>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        initializeInputHolding();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        _animator.SetBool(Animator.StringToHash("IsMoving"), Mathf.Abs(horizontalInput) > 0f || Mathf.Abs(verticalInput) > 0f);


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
        else
        {
            _rb.velocity = Vector2.zero;
        }
    }

    #region Display

    Renderer[] renderers;
    Collider2D[] colliders;
    Rigidbody2D[] rigidbodies;

    public void HideThePlayer()
    {
        renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        rigidbodies = GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D rb in rigidbodies)
        {
            rb.simulated = false;
        }

        enabled = false;
    }

    public void ShowThePlayer()
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = true;
        }

        foreach (Rigidbody2D rb in rigidbodies)
        {
            rb.simulated = true;
        }
    }

    #endregion

    #region Movement

    [SerializeField] private float currentSpeed = 0;
    [SerializeField] private float minSpeed = 2.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    //[SerializeField] private float movementSmoothingSpeed = 1.0f;
    public bool canMove = true;
    Vector2 _rawInputMovement;
    Vector2 _frameVelocity;
    public float Acceleration = 1.2f;
    public float Deceleration = 0.6f;

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
        //Vector3 movement = _rawInputMovement * maxSpeed * Time.deltaTime;
        //_rb.MovePosition(transform.position + movement);
        if (_rawInputMovement.magnitude == 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, minSpeed, Deceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, Acceleration * Time.fixedDeltaTime);
        }
        
        _frameVelocity.x = currentSpeed * _rawInputMovement.normalized.x;
        _frameVelocity.y = currentSpeed * _rawInputMovement.normalized.y;
        _rb.velocity = _frameVelocity;
    }

    #endregion

    #region Input System

    bool isChargingL, isChargingR = false;
    bool isChargingCompletedL, isChargingCompletedR = false;
    float chargingTimeL, chargingTimeR;

    float longPressDuration = 0.3f;
    float maxChargingTime = 1.0f;
    float pressStartTime;

    PlayerInput playerInput;
    InputAction attackActionL, attackActionR;

    Coroutine CheckIsLongPressingCoroutineL, CheckIsChargingCoroutineL;
    Coroutine CheckIsLongPressingCoroutineR, CheckIsChargingCoroutineR;

    void initializeInputHolding()
    {
        attackActionL = playerInput.actions.FindActionMap("Battle").FindAction("AttackL");
        attackActionL.started += OnAttackLStarted => {
            if (CheckIsLongPressingCoroutineR != null) StopCoroutine(CheckIsLongPressingCoroutineR);
            if (CheckIsChargingCoroutineR != null) StopCoroutine(CheckIsChargingCoroutineR);
            pressStartTime = Time.time;
            CheckIsLongPressingCoroutineL = StartCoroutine(CheckIsLongPressingL());
        };
        attackActionL.canceled += OnAttackLCanceled => {
            if (!isChargingCompletedL)
            {
                // canceled while charging not completed
                StopCoroutine(CheckIsLongPressingCoroutineL);
                if (CheckIsChargingCoroutineL != null) StopCoroutine(CheckIsChargingCoroutineL);
                if (isChargingL)
                {
                    _attack.ChargeAttackL(Time.time - chargingTimeL);
                    isChargingL = false;
                }
                else
                {
                    _attack.AttackL();
                }
            }
            else
            {
                isChargingCompletedL = false;
            }
        };

        attackActionR = playerInput.actions.FindActionMap("Battle").FindAction("AttackR");
        attackActionR.started += OnAttackRStarted =>
        {
            if (CheckIsLongPressingCoroutineL != null)  StopCoroutine(CheckIsLongPressingCoroutineL);
            if (CheckIsChargingCoroutineL != null) StopCoroutine(CheckIsChargingCoroutineL);
            pressStartTime = Time.time;
            CheckIsLongPressingCoroutineR = StartCoroutine(CheckIsLongPressingR());
        };
        attackActionR.canceled += OnAttackRCanceled =>
        {
            if (!isChargingCompletedR)
            {
                // canceled while charging not completed
                StopCoroutine(CheckIsLongPressingCoroutineR);
                if (CheckIsChargingCoroutineR != null) StopCoroutine(CheckIsChargingCoroutineR);
                if (isChargingR)
                {
                    _attack.ChargeAttackR(Time.time - chargingTimeR);
                    isChargingR = false;
                }
                else
                {
                    _attack.AttackR();
                }
            }
            else
            {
                isChargingCompletedR = false;
            }
        };
    }

    IEnumerator CheckIsLongPressingL()
    {
        yield return new WaitForSeconds(longPressDuration);
        isChargingL = true;
        _attack.ChargingAttackL();
        if (maxChargingTime > 0)
            CheckIsChargingCoroutineL = StartCoroutine(CheckIsChargingStopL());
    }

    IEnumerator CheckIsChargingStopL()
    {
        chargingTimeL = Time.time;
        yield return new WaitForSeconds(maxChargingTime);
        isChargingL = false;
        isChargingCompletedL = true;
        _attack.ChargeAttackL(maxChargingTime);
    }

    IEnumerator CheckIsLongPressingR()
    {
        yield return new WaitForSeconds(longPressDuration);
        isChargingR = true;
        _attack.ChargingAttackR();
        if (maxChargingTime > 0)
            CheckIsChargingCoroutineR = StartCoroutine(CheckIsChargingStopR());
    }

    IEnumerator CheckIsChargingStopR()
    {
        chargingTimeR = Time.time;
        yield return new WaitForSeconds(maxChargingTime);
        isChargingR = false;
        isChargingCompletedR = true;
        _attack.ChargeAttackR(maxChargingTime);
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        _rawInputMovement = value.ReadValue<Vector2>();
    }

    //public void OnAttack(InputAction.CallbackContext value)
    //{
    //    if (value.started)
    //    {
    //        canMove = false;
    //        _attack.Attack();
    //    }
    //}

    //public void OnSupport(InputAction.CallbackContext value)
    //{
    //    if (value.started)
    //    {
    //        canMove = false;
    //        _attack.Support();
    //    }
    //}

    public void OnInteract(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            _interact.Interact();
        }
    }

    public void OnBackpack(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            if (Backpack.Instance.gameObject.activeSelf)
                Backpack.Instance.Hide();
            else
                Backpack.Instance.Show();
        }
    }

    #endregion

    private void OnDestroy()
    {
        
    }
}
