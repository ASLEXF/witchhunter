using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Singleton<CanvasManager>
{
    public Canvas canvas;

    public float GetCanvasScale()
    {
        return canvas.scaleFactor;
    }
}
