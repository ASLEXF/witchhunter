using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button newGameButton;
    public Button continueButton;
    public Button settingsButton;
    public Button exitButton;
    private Button[] buttons;
    private int currentButtonIndex = -1;

    private bool hasSaveFile = false;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
    }

    private void Start()
    {
        hasSaveFile = ReadSaveFiles().Length > 0;
        if (!hasSaveFile)
        {
            continueButton.interactable = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentButtonIndex == -1)
            {
                currentButtonIndex = 0;
                SelectButton(currentButtonIndex);
            }
            else
            {
                currentButtonIndex = (currentButtonIndex - 1 + buttons.Length) % buttons.Length;
                SelectButton(currentButtonIndex);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentButtonIndex == -1)
            {
                currentButtonIndex = 0;
                SelectButton(currentButtonIndex);
            }
            else
            {
                currentButtonIndex = (currentButtonIndex + 1) % buttons.Length;
                SelectButton(currentButtonIndex);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentButtonIndex >= 0 && buttons[currentButtonIndex] != null && buttons[currentButtonIndex].IsActive())
            {
                buttons[currentButtonIndex].onClick.Invoke();
            }
        }
    }

    public string ReadSaveFiles()
    {
        // TODO: ²éÕÒ´æµµÎÄ¼þ
        return "";
    }

    #region LoadScene

    //private IEnumerator StartNewGame()
    //{
    //    AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync("Main Menu");
    //    yield return unloadOperation;

    //    AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Cliff", LoadSceneMode.Additive);
    //    yield return loadOperation;

    //}


    void SelectButton(int index)
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
            if (!hasSaveFile)
            {
                continueButton.interactable = false;
            }
        }

        buttons[index].Select();
        buttons[index].interactable = false;
    }


    private void StartNewGame()
    {
        //SceneManager.UnloadSceneAsync("Main Menu");
        //StartCoroutine(SceneLoader.Instance.LoadSceneSlowFadeIn("Cliff"));
        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        //SceneManager.LoadScene("Cliff", LoadSceneMode.Additive);
        SceneLoader.Instance.LoadSceneSlowFadeIn("Cliff");
    }

    private void LoadSettingsScene()
    {
        //SceneManager.UnloadSceneAsync("Main Menu");
        //SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
        //StartCoroutine(sceneLoader.LoadScene("Settings"));
        SceneLoader.Instance.LoadScene("Settings");
    }

    #endregion

    public void QuitGame() => Application.Quit();
}
