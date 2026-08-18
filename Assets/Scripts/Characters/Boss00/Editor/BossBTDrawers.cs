using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BossBTCondition))]
public class BossBTConditionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        float y = position.y;
        float w = position.width;
        float h = EditorGUIUtility.singleLineHeight;
        float s = EditorGUIUtility.standardVerticalSpacing;

        SerializedProperty typeProp = property.FindPropertyRelative("type");
        SerializedProperty invertProp = property.FindPropertyRelative("invert");
        EditorGUI.PropertyField(new Rect(position.x, y, w * 0.72f, h), typeProp, GUIContent.none);
        invertProp.boolValue = EditorGUI.ToggleLeft(new Rect(position.x + w * 0.74f, y, w * 0.26f, h), "取反", invertProp.boolValue);
        y += h + s;

        var type = (BossBTConditionType)typeProp.intValue;
        switch (type)
        {
            case BossBTConditionType.PlayerInRange:
            case BossBTConditionType.PlayerOutsideRange:
                EditorGUI.PropertyField(new Rect(position.x, y, w, h), property.FindPropertyRelative("minDistance"), new GUIContent("最小距离"));
                y += h + s;
                EditorGUI.PropertyField(new Rect(position.x, y, w, h), property.FindPropertyRelative("maxDistance"), new GUIContent("最大距离"));
                break;
            case BossBTConditionType.HealthPercentBelow:
            case BossBTConditionType.HealthPercentAbove:
                EditorGUI.PropertyField(new Rect(position.x, y, w, h), property.FindPropertyRelative("healthPercent"), new GUIContent("生命比例"));
                break;
            case BossBTConditionType.HasStorySignal:
                EditorGUI.PropertyField(new Rect(position.x, y, w, h), property.FindPropertyRelative("signalId"), new GUIContent("信号 Id"));
                break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float h = EditorGUIUtility.singleLineHeight;
        float s = EditorGUIUtility.standardVerticalSpacing;
        var type = (BossBTConditionType)property.FindPropertyRelative("type").intValue;
        switch (type)
        {
            case BossBTConditionType.PlayerInRange:
            case BossBTConditionType.PlayerOutsideRange:
                return h * 3 + s * 2;
            case BossBTConditionType.HealthPercentBelow:
            case BossBTConditionType.HealthPercentAbove:
            case BossBTConditionType.HasStorySignal:
                return h * 2 + s;
            default:
                return h;
        }
    }
}

[CustomPropertyDrawer(typeof(BossBTAction))]
public class BossBTActionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        float y = position.y;
        float w = position.width;
        float h = EditorGUIUtility.singleLineHeight;
        float s = EditorGUIUtility.standardVerticalSpacing;

        SerializedProperty typeProp = property.FindPropertyRelative("type");
        EditorGUI.PropertyField(new Rect(position.x, y, w, h), typeProp, GUIContent.none);
        y += h + s;

        var type = (BossBTActionType)typeProp.intValue;
        switch (type)
        {
            case BossBTActionType.Wait:
            case BossBTActionType.Idle:
                EditorGUI.PropertyField(new Rect(position.x, y, w, h), property.FindPropertyRelative("duration"), new GUIContent("持续秒"));
                break;
            case BossBTActionType.ChasePlayer:
                EditorGUI.PropertyField(new Rect(position.x, y, w, h), property.FindPropertyRelative("duration"), new GUIContent("最长秒(0=直到靠近)"));
                y += h + s;
                EditorGUI.PropertyField(new Rect(position.x, y, w, h), property.FindPropertyRelative("moveSpeed"), new GUIContent("移速"));
                y += h + s;
                EditorGUI.PropertyField(new Rect(position.x, y, w, h), property.FindPropertyRelative("stopDistance"), new GUIContent("停止距离"));
                break;
            case BossBTActionType.PlayAnimation:
                EditorGUI.PropertyField(new Rect(position.x, y, w, h), property.FindPropertyRelative("animationName"), new GUIContent("Trigger"));
                y += h + s;
                EditorGUI.PropertyField(new Rect(position.x, y, w, h), property.FindPropertyRelative("duration"), new GUIContent("持续秒"));
                break;
            case BossBTActionType.DrawSword:
            case BossBTActionType.FireBouncyVolley:
                EditorGUI.PropertyField(new Rect(position.x, y, w, h), property.FindPropertyRelative("duration"), new GUIContent("持续秒(0=用组件默认)"));
                break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float h = EditorGUIUtility.singleLineHeight;
        float s = EditorGUIUtility.standardVerticalSpacing;
        var type = (BossBTActionType)property.FindPropertyRelative("type").intValue;
        switch (type)
        {
            case BossBTActionType.ChasePlayer:
                return h * 4 + s * 3;
            case BossBTActionType.PlayAnimation:
                return h * 3 + s * 2;
            case BossBTActionType.Wait:
            case BossBTActionType.Idle:
            case BossBTActionType.DrawSword:
            case BossBTActionType.FireBouncyVolley:
                return h * 2 + s;
            default:
                return h;
        }
    }
}
