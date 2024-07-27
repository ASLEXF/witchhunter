using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAttack : MonoBehaviour
{
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        GameEvents.Instance.OnStoryModeEnd += Show;
        GameEvents.Instance.OnStoryModeStart += Hide;
    }

    private void Show()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void Hide()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
