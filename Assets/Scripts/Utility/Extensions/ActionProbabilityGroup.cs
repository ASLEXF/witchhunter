using System;
using UnityEngine;

[Serializable]
public class ActionProbabilityGroup
{
    [Range(0f, 1f)]
    [SerializeField] public float moveClose = 0.25f;

    [Range(0f, 1f)]
    [SerializeField] public float moveAway = 0.25f;

    [Range(0f, 1f)]
    [SerializeField] public float attack = 0.25f;

    [Range(0f, 1f)]
    [SerializeField] public float wait = 0.25f;

    public float value1
    {
        get { return moveClose; }
        private set {}
    }

    public float value2
    {
        get { return moveClose + moveAway; }
        private set {}
    }

    public float value3
    {
        get { return moveClose + moveAway + attack; }
        private set {}
    }
}