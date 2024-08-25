using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    private static StoryManager _instance;

    public static StoryManager Instance
    {
        get
        {
            if(_instance == null)
            {
                Debug.LogWarning("StoryManager null");
            }
            return _instance;
        }
    }

    public StoryTree StoryTree;

    public bool isPlaying = false;

    [SerializeField] private List<StoryNode> currentNodes;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        InitializeStoryNodes();
    }

    private void Update()
    {
        // check story node condition, begin story mode
        if (GameManager.Instance.isInitiated && !SceneLoader.Instance.isLoading)
        {
            currentNodes = StoryTree.GetCurrentNodes();

            foreach (StoryNode node in currentNodes)
            {
                if (node.CheckConditiion())
                {
                    node.Play();
                    continue;
                }
            }
        }
    }

    void InitializeStoryNodes()
    {
        StoryTree = new StoryTree();
        StoryTree.AddNode(
            new StoryNode(
                "prologue 1",
                "Cliff",
                "Opening",
                "1-1", 
                new Dictionary<string, int>() { { "CurrentSceneName", 3 } },
                true,
                new Vector2(-4.09f, 1.89f)
                ), 
            0
        );
        //StoryTree.AddNode(
        //    new StoryNode(
        //        "prologue 1-2", 
        //        "Cliff", 
        //        "1-2", 
        //        new Dictionary<string, int>() { { "enermyNum", 0 } }
        //        ), 
        //    1
        //);
        //StoryTree.AddNode(
        //    new StoryNode(
        //        "prologue 1-3", 
        //        "Village"
        //        "1-3", 
        //        ), 
        //    2
        //);
    }

    void EndStoryMode()
    {
        PlayerController.Instance.enabled = true;
        isPlaying = false;
        GameEvents.Instance.StoryModeEnded();
    }
}
