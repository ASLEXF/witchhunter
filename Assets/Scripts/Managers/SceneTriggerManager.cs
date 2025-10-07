using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTriggerManager : Singleton<SceneTriggerManager>
{
    private void OnEnable()
    {
        GameEvents.Instance.OnStoryModeEnded += checkCurrentScene;
        GameEvents.Instance.OnMenuClosed += checkCurrentScene;
    }

    private void OnDisable()
    {
        if (GameEvents.HasInstance)
        {
            GameEvents.Instance.OnStoryModeEnded -= checkCurrentScene;
            GameEvents.Instance.OnMenuClosed -= checkCurrentScene;
        }
    }

    void checkCurrentScene()
    {
        foreach (string sceneName in SceneLoader.Instance.ExplorationScenes)
        {
            if (SceneLoader.Instance.CurrentScenes[1] == sceneName)
            {
                GameManager.Instance.StartExplorationMode();
                return;
            }
        }

        foreach (string sceneName in SceneLoader.Instance.BattleScenes)
        {
            if (SceneLoader.Instance.CurrentScenes[1] == sceneName)
            {
                GameManager.Instance.StartBattleMode();
                return;
            }
        }
    }
}
