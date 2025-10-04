using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicSettings : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropDown;
    [SerializeField] Toggle windowed, fullScreen;
    [SerializeField] Button apply;

    [SerializeField] private Resolution[] resolutions;

    private void Start()
    {
        initializeResolutionDropdown();

        apply.onClick.AddListener(() => GraphicManager.Instance.Apply(fullScreen.isOn, resolutions[resolutionDropDown.value]));
    }

    private void initializeResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();

        List<string> options = new List<string>();
        foreach (Resolution res in resolutions)
        {
            options.Add(res.ToString());
        }

        resolutionDropDown.options = options.ConvertAll(options => new TMP_Dropdown.OptionData(options));

        // find value
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (
                resolutions[i].width == GraphicManager.Instance.width
                && resolutions[i].height == GraphicManager.Instance.height
                && resolutions[i].refreshRateRatio.Equals(GraphicManager.Instance.refreshRate)
                )
                resolutionDropDown.value = i;
        }
    }

    public void OnSetWindowedIsOn() => fullScreen.isOn = !windowed.isOn;
    public void OnSetFullScreenIsOn() => windowed.isOn = !fullScreen.isOn;

    private void OnDestroy()
    {
        apply.onClick.RemoveAllListeners();
    }
}
