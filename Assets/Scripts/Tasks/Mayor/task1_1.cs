using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class task1_1 : MonoBehaviour, ITask
{
    NPCInteract interact;

    [SerializeField] bool isActivated = false;
    [SerializeField] int stage = 1;
    [SerializeField] TaskStatus taskStatus = TaskStatus.unpublished;
    [SerializeField] int[] ids = { 1 ,2, 3 };

    public bool isFinished = false;

    private void Awake()
    {
        interact = GetComponent<NPCInteract>();
    }

    private void Start()
    {
    }

    public void SetActive(bool active)
    {
        isActivated = active;
    }

    public bool CheckFinish()
    {
        Item item = PlayerInventory.Instance.FindItem(3);  // fang

        if (item is null)
            return false;

        MaterialItem materialItem = item as MaterialItem;
        if (materialItem.amount > 4)
        {
            materialItem.amount -= 5;
            GameEvents.Instance.ItemsUpdated();
            taskStatus = TaskStatus.finished;
            getReward();
            return true;
        }

        return false;
    }

    private async void getReward()
    {
        // notification
        await PlayerInventory.Instance.AddItem(1, 2);
    }

    public int GetStage()
    {
        return stage;
    }

    public TaskStatus GetTaskStatus()
    {
        return taskStatus;
    }

    public int GetTalkFileId(TaskStatus status)
    {
        switch (status)
        {
            case TaskStatus.published:
            {
                return ids[0];
            }
            case TaskStatus.finish:
            {
                return ids[1];
            }
            case TaskStatus.finished:
            {
                return ids[2];
            }
            default:
            {
                return 0;
            }
        }
    }
}
