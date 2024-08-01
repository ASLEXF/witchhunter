using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMapHandler : MonoBehaviour
{

    [SerializeField] string currentMapName;

    PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        ListenInputEvents();
        SetDefaultInputMap();
    }

    private void Update()
    {
        currentMapName = playerInput.currentActionMap.name;
    }

    void ListenInputEvents()
    {
        GameEvents.Instance.OnMenuOpened += InputActionsToMenu;
        GameEvents.Instance.OnMenuClosed += CheckCurrentScene;
        GameEvents.Instance.OnStoryModeStarted += InputActionsToStory;
        GameEvents.Instance.OnStoryModeEnded += CheckCurrentScene;
        GameEvents.Instance.OnExplorationModeStarted += InputActionsToExploration;
        GameEvents.Instance.OnBattleModeStarted += InputActionsToBattle;
    }
    void SetDefaultInputMap()
    {
        playerInput.SwitchCurrentActionMap("Menu");
    }

    private void CheckCurrentScene()
    {
        Debug.Log($"check current scene {SceneLoader.Instance.CurrentScenes[1]}");
        foreach (string sceneName in SceneLoader.Instance.ExplorationScenes)
        {
            if (SceneLoader.Instance.CurrentScenes[1] == sceneName)
            {
                InputActionsToExploration();
                return;
            }
        }

        foreach (string sceneName in SceneLoader.Instance.BattleScenes)
        {
            if (SceneLoader.Instance.CurrentScenes[1] == sceneName)
            {
                InputActionsToBattle();
                return;
            }
        }
    }

    public void InputActionsToMenu()
    {
        playerInput.SwitchCurrentActionMap("Menu");
        Debug.Log("player input: menu");
    }

    public void InputActionsToStory()
    {
        playerInput.SwitchCurrentActionMap("Story");
        Debug.Log("player input: story");
    }

    public void InputActionsToExploration()
    {
        playerInput.SwitchCurrentActionMap("Exploration");
        Debug.Log("player input: exploration");
    }

    public void InputActionsToBattle()
    {
        playerInput.SwitchCurrentActionMap("Battle");
        Debug.Log("player input: battle");
    }

    private void OnDestroy()
    {
        GameEvents.Instance.OnMenuOpened -= InputActionsToMenu;
        GameEvents.Instance.OnMenuClosed -= CheckCurrentScene;
        GameEvents.Instance.OnStoryModeStarted -= InputActionsToStory;
        GameEvents.Instance.OnStoryModeEnded -= CheckCurrentScene;
        GameEvents.Instance.OnExplorationModeStarted += InputActionsToExploration;
        GameEvents.Instance.OnBattleModeStarted -= InputActionsToBattle;
    }
}
