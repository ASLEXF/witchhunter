using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualRange : MonoBehaviour
{
    [SerializeField] public float distance = 12.0f;  // TODO: maxRange
    [SerializeField][Range(2, 12)] public int lineNum = 6;
    [SerializeField][Range(90, 180)] public float range = 72.0f;

    Vector2 basePosition;
    Vector2 facePosition;  // rotate

    List<LineRenderer> lineRenderers = new List<LineRenderer>();

    private void Update()
    {
        basePosition = transform.position;
        facePosition = transform.GetComponentInParent<NPCController>().facePosition;

        if (facePosition.x * facePosition.y < 0)
        {
            range = 180.0f - range;
        }

        if (lineNum < lineRenderers.Count)
        {
            int count = 0;
            for (; count < lineNum; count++)
            {
                LineRenderer line = lineRenderers[count];

                setLine(line, count);
            }
            for (; count < lineRenderers.Count;)
            {
                LineRenderer line = lineRenderers[count - 1];
                lineRenderers.Remove(line);
                Destroy(line.gameObject);
            }
        }
        else
        {
            int count = 0;
            for (; count < lineRenderers.Count; count++)
            {
                LineRenderer line = lineRenderers[count];

                setLine(line, count);
            }
            for (; count < lineNum; count++)
            {
                GameObject lineObj = new GameObject($"Visial Range Line {count + 1}");
                lineObj.layer = 5;
                lineObj.transform.SetParent(transform);
                LineRenderer line = lineObj.AddComponent<LineRenderer>();

                setLine(line, count);

                lineRenderers.Add(line);
            }
        }
    }

    private void setLine(LineRenderer line, int n)
    {
        line.positionCount = 2;
        line.SetPosition(0, basePosition);
        line.SetPosition(1, basePosition + (new Vector2(1, Mathf.Tan((range / 2 - range / (lineNum - 1) * n) / 180.0f * Mathf.PI)) * facePosition).normalized * distance);
        line.startColor = Color.blue;
        line.endColor = Color.red;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
    }

private void getCollidedPoints()
{}

private void genCollider()
{}

private void display()
{}

}
