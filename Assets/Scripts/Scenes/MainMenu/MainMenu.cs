using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IPointerEnterHandler
{
    public Button newGameButton;
    public Button continueButton;
    public Button settingsButton;
    public Button exitButton;
    private Button[] buttons;
    [SerializeField] private int currentButtonIndex = -1;  // start with -1 to disable this function until get key down

    private bool hasSaveFile = false;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
    }

    private void Start()
    {
        hasSaveFile = readSaveFiles().Length > 0;
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
                selectButton(currentButtonIndex);
            }
            else
            {
                findNextInteractableButton(-1);
                selectButton(currentButtonIndex);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentButtonIndex == -1)
            {
                currentButtonIndex = 0;
                selectButton(currentButtonIndex);
            }
            else
            {
                findNextInteractableButton(1);
                selectButton(currentButtonIndex);
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

    private string readSaveFiles()
    {
        // TODO: 查找存档文件
        // 当存在存档文件时，提供存档选择/覆盖存档时警告玩家
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

    private void findNextInteractableButton(int direction)
    {
        for (currentButtonIndex += direction;; currentButtonIndex += direction)
        {
            currentButtonIndex = currentButtonIndex < 0 ? buttons.Length + currentButtonIndex : currentButtonIndex;
            currentButtonIndex = currentButtonIndex > buttons.Length - 1 ? currentButtonIndex - buttons.Length : currentButtonIndex;
            if (buttons[currentButtonIndex].interactable)
                return;
        }
    }

    private void selectButton(int index)
    {
        //foreach (Button button in buttons)
        //{
        //    button.interactable = true;
        //    if (!hasSaveFile)
        //    {
        //        continueButton.interactable = false;
        //    }
        //}

        buttons[index].Select();
        //buttons[index].interactable = false;
    }


    private void startNewGame()
    {
        //SceneManager.UnloadSceneAsync("Main Menu");
        //StartCoroutine(SceneLoader.Instance.LoadSceneSlowFadeIn("Cliff"));
        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        //SceneManager.LoadScene("Cliff", LoadSceneMode.Additive);
        SceneLoader.Instance.LoadSceneSlowFadeIn("Cliff");
    }

    private void loadSettingsScene()
    {
        //SceneManager.UnloadSceneAsync("Main Menu");
        //SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
        //StartCoroutine(sceneLoader.LoadScene("Settings"));
        SceneLoader.Instance.LoadScene("Settings");
    }

    #endregion

    private void quitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
