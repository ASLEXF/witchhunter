using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class NPCStatusEffect : MonoBehaviour
{
    NPCController controller;

    [SerializeField] NPCLifeStatusEnum lifeStatus;
    [SerializeField] List<DebuffEnum> debuffEnums;
    [SerializeField] List<BuffEnum> buffEnums;

    public bool Dead
    {
        get
        {
            return lifeStatus == NPCLifeStatusEnum.DeadItem || lifeStatus == NPCLifeStatusEnum.DeadEmpty;
        }
    }

    public bool Stunned
    {
        get
        {
            return HasDebuff(DebuffEnum.Stunned);
        }
        set
        {
            if (value)
            {
                debuffEnums.Add(DebuffEnum.Stunned);
            }
            else
            {
                debuffEnums.Remove(DebuffEnum.Stunned);
            }

        }
    }

    public bool Poisoned
    {
        get
        {
            return HasDebuff(DebuffEnum.Poisoned);
        }
        set
        {
            if (value)
            {
                debuffEnums.Add(DebuffEnum.Poisoned);
            }
            else
            {
                debuffEnums.Remove(DebuffEnum.Poisoned);
            }
            
        }
    }

    private void Awake()
    {
        controller = transform.parent.GetComponent<NPCController>();
    }

    private void Start()
    {
        lifeStatus = NPCLifeStatusEnum.Alive;
        debuffEnums = new List<DebuffEnum>();
        buffEnums = new List<BuffEnum>();
    }

    private bool HasDebuff(DebuffEnum debuff)
    {
        foreach(DebuffEnum debuffEnum in debuffEnums)
        {
            if (debuffEnum == debuff) return true;
        }
        return false;
    }

    public void SetDead(float random = 0.8f)
    {
        if (Random.value < random)
        {
            lifeStatus = NPCLifeStatusEnum.DeadItem;
        }
        else
        {
            lifeStatus = NPCLifeStatusEnum.DeadEmpty;
        }
    }
}
