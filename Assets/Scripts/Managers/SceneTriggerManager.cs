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

    [SerializeField] private List<StoryNode> currentNodes;
    

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

    private void Update()
    {
        // check story node condition, begin story mode
        if (GameManager.Instance.isInitiated)
        {
            currentNodes = StoryManager.Instance.StoryTree.GetCurrentNodes();

            foreach (StoryNode node in currentNodes)
            {
                if (node.CheckConditiion())
                {
                    node.Play();
                    break;
                }
            }
        }
    }
}
