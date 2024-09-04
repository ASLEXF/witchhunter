using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffGuideUI : MonoBehaviour
{

    GameObject arrowR;
    GameObject GuideOverlay;

    private void Awake()
    {
        arrowR = transform.GetChild(0).gameObject;
        GuideOverlay = transform.GetChild(1).gameObject;
    }

    public void showGuideUI()
    {
        GuideOverlay.SetActive(true);
    }

    public void hideGuideUI()
    {
        GuideOverlay.SetActive(false);
    }

    public void showArrowR()
    {
        arrowR.SetActive(true);
    }

    public void hideArrowR()
    {
        arrowR.SetActive(false);
    }
}
