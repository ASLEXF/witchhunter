using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
