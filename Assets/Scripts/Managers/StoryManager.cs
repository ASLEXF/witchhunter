using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StoryNode
{
    public static int staticId = -1;

    public int id;
    public string desc;
    public string timeLineFile;
    public string txtFile;
    public string sceneName;
    public Dictionary<string, int> conditions;
    public List<StoryNode> next;

    public bool isMovePosition;
    public Vector2 playerPosition;

    public StoryNode(
        string desc,
        string sceneName,
        string timeLineFile,
        string txtFile = null,
        Dictionary<string, int> conditions = null,

        bool isMovePosition = false,
        Vector2 playerPosition = default(Vector2)
        )
    {
        this.id = staticId++;
        this.desc = desc;
        this.timeLineFile = timeLineFile;
        this.txtFile = txtFile;
        this.sceneName = sceneName;
        this.conditions = conditions;
        this.next = new List<StoryNode>();

        this.isMovePosition = isMovePosition;
        this.playerPosition = playerPosition;
    }

    public bool CheckConditiion()
    {
        if (conditions.Count == 0) return true;

        foreach (KeyValuePair<string, int> dict in this.conditions)
        {
            if (dict.Key == "EnermyNumLessThan")
            {
                if (NPCChecker.Instance.GetEnermyAliveNum() < dict.Value)
                    return true;
            }
            else if (dict.Key == "CloseToNPC")
            {
                if (NPCChecker.Instance.GetInteractableNPC().Contains(dict.Value))
                    return true;
            }
            else if (dict.Key == "CurrentSceneName")
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (scene.IsValid() && scene.buildIndex == dict.Value)
                    {
                        return true;
                    }
                }
            }
            else
            {
                Debug.LogError($"predefined condition {dict.Key} not found!");
                return false;
            }
        }

        return false;
    }

    public void Play()
    {
        // play timeline and enable dialogbox
        if (!StoryManager.Instance.isPlaying)
        {
            StoryManager.Instance.isPlaying = true;
            StoryManager.Instance.StoryTree.RemoveNode(this);

            GameEvents.Instance.StoryModeStarted();

            // dialogbox
            if (txtFile != null)
            {
                DialogBox.Instance.LoadTextAsset(txtFile);
            }

            // timeline
            TimeLineManager.Instance.LoadPlayableAsset(timeLineFile);
            TimeLineManager.Instance.LoadBindingTable(timeLineFile);

            // player controller
            PlayerController.Instance.ShowThePlayer();
            if (isMovePosition) PlayerController.Instance.transform.position = playerPosition;
            PlayerController.Instance.enabled = false;
        }
    }
}
public class StoryTree
{
    public List<StoryNode> nodes;

    public StoryNode root;

    public StoryTree()
    {
        nodes = new List<StoryNode>();
        root = new StoryNode("root", "", "", "");
        nodes.Add(root);
    }

    public void AddNode(StoryNode newNode, int parentId, int childId = 0)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            StoryNode node = nodes[i];
            if (node.id == parentId)
            {
                node.next.Add(newNode);
                nodes.Add(newNode);
                if (childId != 0)
                {
                    List<StoryNode> nodesToRemove = new List<StoryNode>();
                    bool flag = true;
                    foreach (StoryNode childNode in node.next)
                    {
                        if (childNode.id == childId)
                        {
                            newNode.next.Add(childNode);
                            nodesToRemove.Add(childNode);
                            flag = false;
                        }
                    }

                    foreach (StoryNode childNode in nodesToRemove)
                    {
                        node.next.Remove(childNode);
                    }

                    if (flag)
                        Debug.Log("child story node does not exist!");
                }
            }
        }
    }

    public void RemoveNode(StoryNode node)
    {
        //if(id <= 0 || id >= _nodes.Count)
        //{
        //    Debug.Log("invalid id for RemoveNode func!");
        //    return;
        //}

        //if (root.next != null)
        //{
        //    StoryNode previousNode = null;
        //    foreach (StoryNode node in _nodes)
        //    {
        //        if(node.id.Equals(id))
        //        {
        //            if(node.next != null && previousNode != null)
        //            {
        //                previousNode.next = node.next;
        //            }
        //        }
        //        previousNode = node;
        //    }
        //}

        //_nodes.RemoveAll(obj => obj.id == id);

        if (root.next != null)
        {
            StoryNode previousNode = null;
            for (int i = 0; i < this.nodes.Count; i++)
            {
                StoryNode storyNode = this.nodes[i];
                if (storyNode.Equals(node))
                {
                    if (storyNode.next != null && previousNode != null)
                    {
                        previousNode.next = storyNode.next;
                    }
                }
                previousNode = storyNode;
            }

            node.next = null;
            nodes.Remove(node);
        } 
    }

    public List<StoryNode> GetCurrentNodes()
    {
        return root.next;
    }
}

public class StoryManager : MonoBehaviour
{
    private static StoryManager _instance;

    public static StoryManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject singletonObject = new GameObject("StoryManager");
                _instance = singletonObject.AddComponent<StoryManager>();
            }
            return _instance;
        }
    }

    public StoryTree StoryTree = new StoryTree();
    public bool isPlaying = false;

    [SerializeField] List<StoryNode> _nodes;

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
        _nodes = StoryTree.nodes;
    }

    void InitializeStoryNodes()
    {
        StoryTree.AddNode(
            new StoryNode(
                "prologue 1",
                "Cliff",
                "Opening",
                "1-1", 
                new Dictionary<string, int>() { { "CurrentSceneName", 3 } },
                true,
                new Vector2(16.05f, -7f)
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

    public void EndStoryMode()
    {
        PlayerController.Instance.enabled = true;
        isPlaying = false;
        GameEvents.Instance.StoryModeEnded();
    }
}
