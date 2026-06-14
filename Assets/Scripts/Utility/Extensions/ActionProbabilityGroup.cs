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
}