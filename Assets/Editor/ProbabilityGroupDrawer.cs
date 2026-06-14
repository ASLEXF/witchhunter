#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ActionProbabilityGroup))]
public class ProbabilityGroupDrawer : PropertyDrawer
{
    private static float LineHeight =>
        EditorGUIUtility.singleLineHeight;

    private static float LineSpacing =>
        EditorGUIUtility.standardVerticalSpacing;

    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty value1 =
            property.FindPropertyRelative("moveClose");

        SerializedProperty value2 =
            property.FindPropertyRelative("moveAway");

        SerializedProperty value3 =
            property.FindPropertyRelative("attack");

        SerializedProperty value4 =
            property.FindPropertyRelative("wait");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float currentY = position.y;

        EditorGUI.LabelField(
            new Rect(
                position.x,
                currentY,
                position.width,
                lineHeight), 
            label, 
            EditorStyles.boldLabel);

        currentY += LineHeight + LineSpacing;

        EditorGUI.indentLevel++;

        DrawWeightField(
            GetChildRect(position, currentY),
            value1,
            new GUIContent("Move Close"),
            0,
            value1,
            value2,
            value3,
            value4);

        currentY += LineHeight + LineSpacing;

        DrawWeightField(
            GetChildRect(position, currentY),
            value2,
            new GUIContent("Move Away"),
            1,
            value1,
            value2,
            value3,
            value4);

        currentY += LineHeight + LineSpacing;

        DrawWeightField(
            GetChildRect(position, currentY),
            value3,
            new GUIContent("Attack"),
            2,
            value1,
            value2,
            value3,
            value4);

        currentY += LineHeight + LineSpacing;

        DrawWeightField(
            GetChildRect(position, currentY),
            value4,
            new GUIContent("Wait"),
            3,
            value1,
            value2,
            value3,
            value4);

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    private static Rect GetChildRect(
        Rect position,
        float y)
    {
        Rect rect = new Rect(
            position.x,
            y,
            position.width,
            LineHeight);

        int previousIndent = EditorGUI.indentLevel;

        EditorGUI.indentLevel = previousIndent + 1;
        rect = EditorGUI.IndentedRect(rect);
        EditorGUI.indentLevel = previousIndent;

        return rect;
    }

    private static void DrawWeightField(
        Rect position,
        SerializedProperty editedProperty,
        GUIContent label,
        int editedIndex,
        params SerializedProperty[] properties)
    {
        EditorGUI.BeginChangeCheck();

        float newValue = EditorGUI.Slider(
            position,
            label,
            editedProperty.floatValue,
            0f,
            1f);

        if (!EditorGUI.EndChangeCheck())
        {
            return;
        }

        editedProperty.floatValue = Mathf.Clamp01(newValue);

        NormalizeKeepingEditedValue(
            properties,
            editedIndex);
    }

    private static void NormalizeKeepingEditedValue(
        SerializedProperty[] properties,
        int editedIndex)
    {
        float editedValue =
            Mathf.Clamp01(properties[editedIndex].floatValue);

        properties[editedIndex].floatValue = editedValue;

        float remainingValue = 1f - editedValue;
        float otherTotal = 0f;

        for (int i = 0; i < properties.Length; i++)
        {
            if (i == editedIndex)
            {
                continue;
            }

            properties[i].floatValue =
                Mathf.Clamp01(properties[i].floatValue);

            otherTotal += properties[i].floatValue;
        }

        if (otherTotal > Mathf.Epsilon)
        {
            float scale = remainingValue / otherTotal;

            for (int i = 0; i < properties.Length; i++)
            {
                if (i == editedIndex)
                {
                    continue;
                }

                properties[i].floatValue *= scale;
            }
        }
        else
        {
            float equalValue =
                remainingValue / (properties.Length - 1);

            for (int i = 0; i < properties.Length; i++)
            {
                if (i == editedIndex)
                {
                    continue;
                }

                properties[i].floatValue = equalValue;
            }
        }

        CorrectFloatingPointError(properties, editedIndex);
    }

    private static void CorrectFloatingPointError(
        SerializedProperty[] properties,
        int editedIndex)
    {
        float total = 0f;

        for (int i = 0; i < properties.Length; i++)
        {
            total += properties[i].floatValue;
        }

        float difference = 1f - total;

        for (int i = properties.Length - 1; i >= 0; i--)
        {
            if (i == editedIndex)
            {
                continue;
            }

            properties[i].floatValue =
                Mathf.Clamp01(
                    properties[i].floatValue + difference);

            break;
        }
    }

    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        return LineHeight * 5f +
               LineSpacing * 4f;
    }
}

#endif