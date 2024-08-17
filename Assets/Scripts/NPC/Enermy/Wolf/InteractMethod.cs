using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractMethod : MonoBehaviour
{
    private NPCStatus status;



    private void Awake()
    {
        status = GetComponent<NPCStatus>();
    }

    public void Interact()
    {
        switch(status)
        {
            case NPCStatus.Alive:
                Talk();
                break;
            case NPCStatus.Dead:
                GetNPCItem();
                break;
            case NPCStatus.DeadEmpty:
                PickUpBody();
                break;
        }
    }

    private void Talk()
    {

    }

    private void GetNPCItem()
    {
        
    }

    private void PickUpBody()
    {

    }
}
