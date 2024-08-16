using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] GameObject biteTrigger;

    public void EnableBiteTrigger()
    {
        biteTrigger.SetActive(true);
    }

    public void DisableBiteTrigger()
    {
        biteTrigger.SetActive(false);
    }
}
