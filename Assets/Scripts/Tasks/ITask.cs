# nullable enable

public interface ITask
{
    public bool CheckFinish();

    public TaskStatus GetTaskStatus();

    public int GetTalkFileId(TaskStatus status);
}
