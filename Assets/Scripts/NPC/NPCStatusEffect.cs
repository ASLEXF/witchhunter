using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class NPCStatusEffect : MonoBehaviour
{
    public string NPCName;

    NPCController controller;
    NPCInteract NPCInteract;

    public NPCLifeStatusEnum lifeStatus;
    [SerializeField] List<DebuffEnum> debuffEnums;
    [SerializeField] List<BuffEnum> buffEnums;

    public bool Alive
    {
        get
        {
            return lifeStatus == NPCLifeStatusEnum.Alive;
        }
    }
    public bool Dead
    {
        get
        {
            return lifeStatus == NPCLifeStatusEnum.DeadItem || lifeStatus == NPCLifeStatusEnum.DeadEmpty;
        }
    }
    public bool DeadItem
    {
        get
        {
            return lifeStatus == NPCLifeStatusEnum.DeadItem;
        }
    }
    public bool DeadEmpty
    {
        get
        {
            return lifeStatus == NPCLifeStatusEnum.DeadEmpty;
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

    public async void SetDead(float random = 0.8f)
    {
        if (Random.value < random)
        {
            lifeStatus = NPCLifeStatusEnum.DeadItem;
            await NPCInteract.GenerateItems(NPCInteract.raceName);
        }
        else
        {
            lifeStatus = NPCLifeStatusEnum.DeadEmpty;
        }
    }
}
