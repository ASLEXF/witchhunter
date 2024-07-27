using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private int status = 0;



    private void Awake()
    {
        
    }

    public void interact()
    {
        switch(this.status)
        {
            case 0:
                break;
            case 1:
                GetNPCItem();
                break;
            case 2:
                PickUpBody();
                break;
            default:
                Debug.Log("status wrong" + this.status);
                break;
        }
    }

    private void GetNPCItem()
    {
        this.status = 2;
    }

    private void PickUpBody()
    {

    }
}
