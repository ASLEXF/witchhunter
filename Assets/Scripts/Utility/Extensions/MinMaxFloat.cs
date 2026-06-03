using UnityEngine;

[System.Serializable]
public class MinMaxFloat
{
    [Min(0f)]
    public float min = 0f;

    [Min(0f)]
    public float max = 1f;

    public float Value
    {
        get
        {
            return _getRandomValue();
        }
    }

    public void CheckValidity(string fieldName, Object owner)
    {
        if (!_isValid())
        {
            Debug.LogError($"Invalid MinMaxFloat values for {fieldName} in {owner.name}. Ensure that min <= max and min >= 0.");
        }
    }

    private bool _isValid()
    {
        return min <= max && min >= 0f;
    }

    private float _getRandomValue()
    {
        if (!_isValid())
            return 0f;
        return Random.Range(min, max);
    }
}