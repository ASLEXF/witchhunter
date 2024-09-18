using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    public Canvas canvas;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public float GetCanvasScale()
    {
        return canvas.scaleFactor;
    }
}
