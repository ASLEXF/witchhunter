using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAttack : MonoBehaviour
{
    GameObject panel;

    private void Awake()
    {
        panel = transform.GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnStoryModeEnded += Show;
            GameEvents.Instance.OnStoryModeStarted += Hide;
        }
    }

    private void OnDisable()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnStoryModeEnded -= Show;
            GameEvents.Instance.OnStoryModeStarted -= Hide;
        }
    }

    private void Start()
    {
        Hide();
    }

    private void Show()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    private void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
