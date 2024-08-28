using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    [SerializeField] private List<bool> isInteractable;
    [SerializeField] private Vector3 UIOffset;
    private NPCLifeStatusEnum status;

    private void Awake()
    {
        
    }

    private void Start()
    {
        isInteractable = new List<bool> ();
    }

    public void Interacted()
    {
        status = NPCLifeStatusEnum.Alive;
        if (isInteractable[((int)status)])
        {
            switch (status)
            {
                case NPCLifeStatusEnum.Alive:
                    Talk();
                    break;
                case NPCLifeStatusEnum.DeadItem:
                    GetNPCItem();
                    break;
                case NPCLifeStatusEnum.DeadEmpty:
                    PickUpBody();
                    break;
            }
        }
    }

    private void Talk()
    {
        Debug.Log("NPC Talk");
    }

    private void GetNPCItem()
    {
        Debug.Log("NPC GetNPCItem");
    }

    private void PickUpBody()
    {
        Debug.Log("NPC PickUpBody");
    }
}
