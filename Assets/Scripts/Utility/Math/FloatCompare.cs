using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatCompare : MonoBehaviour
{
    public static bool IsApproximately(float a, float b, float threshold)
    {
        return Mathf.Abs(a - b) < threshold;
    }
}
