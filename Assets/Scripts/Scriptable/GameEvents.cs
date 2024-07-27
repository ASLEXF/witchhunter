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
                _instance = new GameEvents();
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
        _instance = this;
    }

    #region Game Mode Change

    public event Action OnMenuOpened;
    public event Action OnMenuClosed;
    public event Action OnStoryModeStart;
    public event Action OnStoryModeEnd;
    public event Action OnExplorationModeStart;
    public event Action OnBattleModeStart;

    public void MenuOpened()
    {
        if (OnMenuOpened != null)
            OnMenuOpened();
    }

    public void MenuClosed()
    {
        if(OnMenuClosed != null)
            OnMenuClosed();
    }

    public void StoryModeStart()
    {
        if (OnStoryModeStart != null)
            OnStoryModeStart();
    }

    public void StoryModeEnd()
    {
        if (OnStoryModeEnd != null)
            OnStoryModeEnd();
    }

    public void ExplorationModeStart()
    {
        if (OnExplorationModeStart != null)
            OnExplorationModeStart();
    }

    public void BattleModeStart()
    {
        if (OnBattleModeStart != null)
            OnBattleModeStart();
    }

    #endregion

    #region Dialog Box

    public event Action OnTextScriptUpdate;
    public event Action OnDialogBoxStart;
    public event Action OnDialogBoxEnd;

    public void TextScriptUpdate()
    {
        if (OnTextScriptUpdate != null)
            OnTextScriptUpdate();
    }

    public void DialogBoxStart()
    {
        if (OnDialogBoxStart != null)
            OnDialogBoxStart();
    }

    public void DialogBoxEnd()
    {
        if (OnDialogBoxEnd != null)
            OnDialogBoxEnd();
    }

    #endregion

    #region Timeline

    public event Action OnAssetLoaded;

    public void AssetLoaded()
    {
        if (OnAssetLoaded != null) OnAssetLoaded();
    }

    #endregion
}
