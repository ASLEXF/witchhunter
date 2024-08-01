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
                GameObject obj = new GameObject("GameEvents");
                _instance = obj.AddComponent<GameEvents>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != this)
        {
            Destroy(_instance);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
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
    public event Action OnDialogBoxStarted;
    public event Action OnDialogBoxEnded;

    public void TextScriptUpdated() => OnTextScriptUpdated?.Invoke();

    public void DialogBoxStarted() => OnDialogBoxStarted?.Invoke();

    public void DialogBoxEnded() => OnDialogBoxEnded?.Invoke();

    #endregion

    #region Timeline

    public event Action OnAssetLoaded;

    public void AssetLoaded() => OnAssetLoaded?.Invoke();

    #endregion
}
