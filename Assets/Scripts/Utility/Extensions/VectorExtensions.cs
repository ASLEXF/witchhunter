using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector2 RandomVector()
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}

public static class Vector2Utility
{
    public static Vector2 RotateTowards(
        Vector2 current,
        Vector2 target,
        float maxRadiansDelta
    )
    {
        if (current.sqrMagnitude < 0.0001f)
            return target.normalized;

        if (target.sqrMagnitude < 0.0001f)
            return current.normalized;

        float currentAngle = Mathf.Atan2(current.y, current.x);
        float targetAngle = Mathf.Atan2(target.y, target.x);

        float newAngle = Mathf.MoveTowardsAngle(
            currentAngle * Mathf.Rad2Deg,
            targetAngle * Mathf.Rad2Deg,
            maxRadiansDelta * Mathf.Rad2Deg
        ) * Mathf.Deg2Rad;

        return new Vector2(
            Mathf.Cos(newAngle),
            Mathf.Sin(newAngle)
        );
    }
}
