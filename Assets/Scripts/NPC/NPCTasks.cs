# nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTasks : MonoBehaviour
{
    public ITask? GetTaskByStatus(TaskStatus status)
    {
        ITask[] tasks = GetComponents<ITask>();
        for (int i = 0; i < tasks.Length; i++)
        {
            if (tasks[i] != null && tasks[i].GetTaskStatus() == status)
            {
                return tasks[i];
            }
        }

        return null;
    }
}
