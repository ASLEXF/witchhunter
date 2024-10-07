using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    public bool isInteractable;
    [SerializeField] public Vector3 UIOffset = new Vector3(0f, 0.5f, 0f);

    public virtual void Interacted()
    {
    }
}
