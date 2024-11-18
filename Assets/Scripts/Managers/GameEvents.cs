using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    private static GameEvents _instance;

    public static GameEvents Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogWarning("game events null");
                GameObject obj = new GameObject("GameEvents");
                _instance = obj.AddComponent<GameEvents>();
            }
            return _instance;
        }
    }

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(_instance);
        }
    }

    #region Game Mode

    public event Action OnMenuOpened;
    public event Action OnMenuClosed;
    public event Action OnStoryModeStarted;
    public event Action OnStoryModeEnded;
    public event Action OnExplorationModeStarted;
    public event Action OnBattleModeStarted;

    public void MenuOpened() => OnMenuOpened?.Invoke();

    public void MenuClosed() => OnMenuClosed?.Invoke();

    public void StoryModeStarted() => OnStoryModeStarted?.Invoke();

    public void StoryModeEnded() => OnStoryModeEnded?.Invoke();

    public void ExplorationModeStarted() => OnExplorationModeStarted?.Invoke();

    public void BattleModeStarted() => OnBattleModeStarted?.Invoke();

    #endregion

    #region Dialog Box

    public event Action OnTextScriptUpdated;
    public event Action OnDialogBoxStart;
    public event Action OnDialogBoxStarted;
    public event Action OnDialogBoxEnded;

    public void TextScriptUpdated() => OnTextScriptUpdated?.Invoke();

    public void DialogBoxStart() => OnDialogBoxStart?.Invoke();

    public void DialogBoxStarted() => OnDialogBoxStarted?.Invoke();

    public void DialogBoxEnded() => OnDialogBoxEnded?.Invoke();

    #endregion

    #region Scene

    public event Action OnNewSceneLoaded;

    public void NewSceneLoaded() => OnNewSceneLoaded?.Invoke();

    #endregion

    #region Timeline

    public event Action OnPlayableAssetLoaded;

    public void PlayableAssetLoaded() => OnPlayableAssetLoaded?.Invoke();

    #endregion

    #region Player Status

    public event Action OnPlayerHealthChanged;

    public void PlayerHealthChanged() => OnPlayerHealthChanged?.Invoke();

    #endregion

    #region Interact

    public event Action OnInteractableUpdated;

    public void InteractableUpdated() => OnInteractableUpdated?.Invoke();

    #endregion

    #region Items

    public event Action OnItemsUpdated;

    public event Action OnAddItemWhenInventoryFull;

    public void ItemsUpdated() => OnItemsUpdated?.Invoke();

    public void AddItemWhenInventoryFull() => OnAddItemWhenInventoryFull?.Invoke();

    #endregion

}
