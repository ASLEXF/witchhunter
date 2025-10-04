using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Header("Grapgic")]
    public GameObject GrapgicSettings;
    public GameObject Resolution;
    Dropdown ResolutionDropdown;
    public Toggle WindowedToggle, FullScreenToggle;

    [Header("Input")]
    public GameObject InputSettings;

    [Header("Audio")]
    public GameObject AudioSettings;

    private void Awake()
    {
        ResolutionDropdown = Resolution.GetComponent<Dropdown>();
    }

    private void Start()
    {
        OnGraphicSettings();
    }

    #region Menu

    public void OnGraphicSettings()
    {
        GrapgicSettings.SetActive(true);
        InputSettings.SetActive(false);
        AudioSettings.SetActive(false);
    }

    public void OnAudioSettings()
    {
        GrapgicSettings.SetActive(false);
        InputSettings.SetActive(false);
        AudioSettings.SetActive(true);
    }

    public void OnInputSettings()
    {
        GrapgicSettings.SetActive(false);
        InputSettings.SetActive(true);
        AudioSettings.SetActive(false);
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("Main Menu");
    }

    #endregion

    #region Graphic

    public void OnResolutionChanged()
    {
        // ResolutionDropdown.value
    }

    public void OnWindowedIsOn()
    {
        FullScreenToggle.isOn = !WindowedToggle.isOn;
    }

    public void OnFullScreenIsOn()
    {
        WindowedToggle.isOn = !FullScreenToggle.isOn;
    }

    public void OnGraphicApply()
    {

    }

    #endregion

    #region Input

    public void OnInputChanged()
    {

    }

    #endregion

    #region Audio

    public void OnAudioChanged()
    {

    }

    #endregion
}
