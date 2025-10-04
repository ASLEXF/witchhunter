using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionExample : MonoBehaviour
{
    public Resolution resolution = new Resolution();
}

public class GraphicManager : Singleton<GraphicManager>
{
    [Header("Current")]
    [SerializeField] public int width;
    [SerializeField] public int height;
    [SerializeField] public RefreshRate refreshRate;
    [SerializeField] bool isFullScreen = false;

    [Header("New")]
    Resolution newResolution;
    [SerializeField] bool newIsFullScreen;

    private void Start()
    {
        // load saved settings
        width = Screen.width;
        height = Screen.height;
        refreshRate = Screen.currentResolution.refreshRateRatio;
    }

    private Resolution FindClosestResolution(Resolution current, Resolution[] supported)
    {
        Resolution bestMatch = supported[0];
        int smallestDifference = int.MaxValue;

        foreach (Resolution res in supported)
        {
            int resolutionDifference = Mathf.Abs(res.width - current.width) + Mathf.Abs(res.height - current.height);

            int refreshRateDifference = Mathf.Abs(res.refreshRate - current.refreshRate);

            int totalDifference = resolutionDifference + refreshRateDifference;  // roughly

            if (totalDifference < smallestDifference)
            {
                smallestDifference = totalDifference;
                bestMatch = res;
            }
        }

        return bestMatch;
    }

    public void Apply(bool isFullScreen, Resolution res)
    {
        newResolution = res;
        newIsFullScreen = isFullScreen;

        if (isFullScreen)
            Screen.SetResolution(res.width, res.height, FullScreenMode.MaximizedWindow, res.refreshRateRatio);
        else
            Screen.SetResolution(res.width, res.height, FullScreenMode.Windowed, res.refreshRateRatio);

        GameEvents.Instance.ApplyGraphicSettings();
    }

    public void Confirm()
    {
        width = newResolution.width;
        height = newResolution.height;
        refreshRate = newResolution.refreshRateRatio;
        isFullScreen = newIsFullScreen;
    }

    public void Revert()
    {
        if (isFullScreen)
            Screen.SetResolution(width, height, FullScreenMode.MaximizedWindow, refreshRate);
        else
            Screen.SetResolution(width, height, FullScreenMode.Windowed, refreshRate);
    }
}
