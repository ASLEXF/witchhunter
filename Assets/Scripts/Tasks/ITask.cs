# nullable enable

public interface ITask
{
    public void SetActive(bool active);

    public bool CheckFinish();

    public int GetStage();

    public TaskStatus GetTaskStatus();

    public int GetTalkFileId(TaskStatus status);
}
