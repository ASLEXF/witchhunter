# nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTasks : MonoBehaviour
{
    public bool hasTask = false;

    public ITask? GetTaskByStatus(TaskStatus status, int stage = 1)
    {
        ITask[] tasks = GetComponents<ITask>();
        for (int i = 0; i < tasks.Length; i++)
        {
            if (tasks[i] != null && tasks[i].GetStage() == stage && tasks[i].GetTaskStatus() == status)
            {
                return tasks[i];
            }
        }

        return null;
    }

    public bool ActivateTasksByStage()
    {
        ITask? task = GetTaskByStatus(TaskStatus.unpublished, 1);
        if (task != null)
        {
            task.SetActive(true);
            return true;
        }

        return false;
    }
}
