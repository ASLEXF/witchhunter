using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

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
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        GameEvents.Instance.OnStoryModeStarted += InputActionsToStory;
        GameEvents.Instance.OnStoryModeEnded += ResumeInputActions;

        ListenInputEvents();

        SetDefaultInputMap();
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

    [SerializeField] PlayerInput _playerInput;
    [SerializeField] Actions _actions;
    string _formerActionMapName;

    void SetDefaultInputMap()
    {
        _playerInput.SwitchCurrentActionMap("Menu");
    }

    void ListenSceneLoaded()
    {
        SceneManager.sceneLoaded += (scene, mode) => 
        {
            foreach (string sceneName in SceneLoader.Instance.explorationScenes)
            {
                if (scene.name == sceneName)
                {
                    InputActionsToExploration();
                    return;
                }
            }

            foreach (string sceneName in SceneLoader.Instance.battleScenes)
            {
                if (scene.name == sceneName)
                {
                    InputActionsToBattle();
                    return;
                }
            }
        };
    }

    void ListenInputEvents()
    {
        GameEvents.Instance.OnMenuOpened += InputActionsToMenu;
        GameEvents.Instance.OnMenuClosed += ResumeInputActions;
        GameEvents.Instance.OnStoryModeStarted += InputActionsToStory;
        GameEvents.Instance.OnStoryModeEnded += ResumeInputActions;
        GameEvents.Instance.OnExplorationModeStarted += InputActionsToExploration;
        GameEvents.Instance.OnBattleModeStarted += InputActionsToBattle;
    }

    void ResumeInputActions()
    {
        _playerInput.SwitchCurrentActionMap(_formerActionMapName);
    }

    public void InputActionsToMenu()
    {
        _formerActionMapName = _playerInput.currentActionMap.name;
        _playerInput.SwitchCurrentActionMap("Menu");
    }

    public void InputActionsToStory()
    {
        _formerActionMapName = _playerInput.currentActionMap.name;
        _playerInput.SwitchCurrentActionMap("Story");
    }

    public void InputActionsToExploration()
    {
        _playerInput.SwitchCurrentActionMap("Exploration");
    }

    public void InputActionsToBattle()
    {
        _playerInput.SwitchCurrentActionMap("Battle");
    }

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

    #region Animation
    void CanMove() => canMove = true;

    #endregion

    #region Item

    public void GetItem(int ItemID)
    {

    }

    #endregion
}
