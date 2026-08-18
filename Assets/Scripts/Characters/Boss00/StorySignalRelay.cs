using UnityEngine;

/// <summary>
/// Timeline Signal Receiver / 剧情节点可调用，向全场广播剧情信号。
/// </summary>
public class StorySignalRelay : MonoBehaviour
{
    [SerializeField] string signalId = "Boss00_DrawSword";

    public void Emit()
    {
        Emit(signalId);
    }

    public void Emit(string id)
    {
        if (string.IsNullOrEmpty(id))
            return;

        GameEvents.Instance.StorySignal(id);
    }
}
