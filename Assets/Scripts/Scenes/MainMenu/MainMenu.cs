using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private int currentButtonIndex = -1;  // start with -1 to disable this function until get key down
    private ButtonScript[] buttons;
    private bool hasSaveFile = false;

    [SerializeField] private InputActionReference navigateAction;
    [SerializeField] private InputActionReference submitAction;
    private float inputCooldown = 0.2f;
    private float timer;

    private void Awake()
    {
        buttons = GetComponentsInChildren<ButtonScript>();
    }

    private void Start()
    {
        hasSaveFile = readSaveFiles().Length > 0;
        if (!hasSaveFile)
        {
            buttons[1].Interactable = false;
        }
    }

    private void Update()
    {
        timer -= Time.unscaledDeltaTime;

        float nav = navigateAction.action.ReadValue<float>();

        if (timer <= 0f)
        {
            if (nav > 0.5f)
            {
                MoveSelection(-1);
                timer = inputCooldown;
            }
            else if (nav < -0.5f)
            {
                MoveSelection(1);
                timer = inputCooldown;
            }
        }

        if (submitAction.action.WasPressedThisFrame())
        {
            if (currentButtonIndex >= 0)
                buttons[currentButtonIndex].Invoke();
        }
    }

    private void OnEnable()
    {
        navigateAction.action.Enable();
        submitAction.action.Enable();
        GameEvents.Instance.OnButtonHover += Instance_OnButtonHover;
    }

    private void OnDisable()
    {
        navigateAction.action.Disable();
        submitAction.action.Disable();
        if (GameEvents.HasInstance)
        {
            GameEvents.Instance.OnButtonHover -= Instance_OnButtonHover;
        }
    }

    private string readSaveFiles()
    {
        // TODO: 查找存档文件
        // 当存在存档文件时，提供存档选择/覆盖存档时警告玩家
        return "";
    }

    #region Button selection

    private void MoveSelection(int direction)
    {
        if (currentButtonIndex == -1)
        {
            currentButtonIndex = 0;
        }
        else
        {
            findNextInteractableButton(direction);
        }

        selectButton(currentButtonIndex);
    }

    private void findNextInteractableButton(int direction)
    {
        for (currentButtonIndex += direction; ; currentButtonIndex += direction)
        {
            currentButtonIndex = currentButtonIndex < 0 ? buttons.Length + currentButtonIndex : currentButtonIndex;
            currentButtonIndex = currentButtonIndex > buttons.Length - 1 ? currentButtonIndex - buttons.Length : currentButtonIndex;
            if (buttons[currentButtonIndex].Interactable)
                return;
        }
    }

    private void selectButton(int index) => buttons[index].Select();

    private void Instance_OnButtonHover(int index)
    {
        currentButtonIndex = index;
        selectButton(index);
    }

    #endregion

    #region LoadScene

    public void StartNewGame()
    {
        SceneLoader.Instance.LoadSceneSlowFadeIn("Cliff");
    }

    public void LoadSettingsScene()
    {
        SceneManager.LoadScene("Settings");
    }

    #endregion

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
