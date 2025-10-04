using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOrderLogger : MonoBehaviour
{
    void OnDestroy()
    {
        Debug.Log($"[OnDestroy] {gameObject.name} destroyed at frame {Time.frameCount}");
    }
}
