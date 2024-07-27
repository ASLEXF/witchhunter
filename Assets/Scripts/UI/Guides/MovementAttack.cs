using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAttack : MonoBehaviour
{
    private void Start()
    {
        GameEvents.Instance.OnStoryModeEnded += Show;
        GameEvents.Instance.OnStoryModeStarted += Hide;

        Hide();
    }

    private void Show()
    {
        if (transform.childCount > 0)
            transform.GetChild(0).gameObject.SetActive(true);
    }

    private void Hide()
    {
        if (transform.childCount > 0)
            transform.GetChild(0).gameObject.SetActive(false);
    }
}
