using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        this.id = ++staticId;
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
