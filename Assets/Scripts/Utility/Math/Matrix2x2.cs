using UnityEngine;

public class Matrix2x2
{
    private float[,] values;

    public Matrix2x2(float a, float b, float c, float d)
    {
        values = new float[,] { { a, b }, { c, d } };
    }

    public Matrix2x2(float degree)
    {
        values = new float[,] { { Mathf.Cos(degree), -Mathf.Sin(degree) }, { Mathf.Sin(degree), Mathf.Cos(degree) } };
    }

    public static Vector2 operator *(Matrix2x2 matrix, Vector2 vector)
    {
        float x = matrix.values[0, 0] * vector.x + matrix.values[0, 1] * vector.y;
        float y = matrix.values[1, 0] * vector.x + matrix.values[1, 1] * vector.y;
        return new Vector2(x, y);
    }

    public float getDegree()
    {
        return Mathf.Acos((this * new Vector2(1, 1)).x / Mathf.Sqrt(2)) + Mathf.PI / 4;
    }
}
