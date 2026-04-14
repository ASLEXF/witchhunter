#nullable enable

[System.Serializable]
public struct AnimationStruc
{
    public string? triggerName;
}

[System.Serializable]
public struct WeaponAnimationStructure
{
    public bool hasAttack;
    public int damage;
    public int comboCount;
    public string? triggerName;
    public string? integerName;
    public AttackCondition? attackCondition;
}