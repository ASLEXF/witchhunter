using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTriggerManager : MonoBehaviour
{
    private static SceneTriggerManager _instance;

    public static SceneTriggerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("SceneTriggerManager");
                _instance = singletonObject.AddComponent<SceneTriggerManager>();
            }
            return _instance;
        }
    }

    
    

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        GameEvents.Instance.OnStoryModeEnded += checkCurrentScene;
        GameEvents.Instance.OnMenuClosed += checkCurrentScene;
    }

    private void Update()
    {
        
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
