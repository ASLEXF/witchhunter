using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;

    public static SceneLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("SceneLoader");
                _instance = singletonObject.AddComponent<SceneLoader>();
            }
            return _instance;
        }
    }

    public List<string> BattleScenes = new List<string>() { "Cliff", "Forest" };
    public List<string> ExplorationScenes = new List<string>() { "Village", "Room", "Basement" };

    public Animator fadeAnimator;

    public List<string> CurrentScenes = new List<string>();  // FIXME
    public bool isLoading = true;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("destroy sceneLoader");
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public IEnumerator GameSceneInitiate()
    {
        StartCoroutine(UnloadCurrentScenes());
        LoadScene("Main Menu");

        yield return null;

        isLoading = false;
    }

    IEnumerator UnloadCurrentScenes()
    {
        int sceneCount = SceneManager.sceneCount;

        for (int i = sceneCount - 1; i >= 0; i--)
        {
            UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == "Persistent")
            {
                CurrentScenes.Add(scene.name);
                continue;
            }
            if (scene.isLoaded)
            {
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
            }
        }

        yield break;
    }

    public void LoadScene(string sceneName)
    {
        if (CurrentScenes.Contains(sceneName)) { return; }
        else if (CurrentScenes.Count > 1)
        {
            SceneManager.UnloadSceneAsync(CurrentScenes[1]);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            CurrentScenes.Remove(CurrentScenes[1]);
            CurrentScenes.Add(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            CurrentScenes.Add(sceneName);
        }
    }

    public void LoadSceneSlowFadeIn(string sceneName)
    {
        if(CurrentScenes.Contains(sceneName)) { return; }
        else if(CurrentScenes.Count > 1)
        {
            SceneManager.UnloadSceneAsync(CurrentScenes[1]);
            StartCoroutine(loadSceneSlowFadeIn(sceneName));
            CurrentScenes.Remove(CurrentScenes[1]);
            CurrentScenes.Add(sceneName);
        }
        else
        {
            StartCoroutine(loadSceneSlowFadeIn(sceneName));
            CurrentScenes.Add(sceneName);
        }
    }

    //public void SetCurrentScene(string sceneName) => currentSceneName = sceneName;

    private IEnumerator loadSceneFade(string sceneName)
    {
        if (isLoading)
            yield return null;
        else
        {
            isLoading = true;

            if (fadeAnimator != null && fadeAnimator.isActiveAndEnabled)
            {
                fadeAnimator.SetBool("FadeIn", false);
                fadeAnimator.SetBool("SlowFadeIn", false);
                fadeAnimator.SetBool("FadeOut", true);
                fadeAnimator.SetBool("SlowFadeOut", false);
            }
            else
            {
                Debug.Log("animator fail " + fadeAnimator.isActiveAndEnabled);
            }

            yield return new WaitForSeconds(1);

            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            //async.completed += UnloadCurrentScene;
            async.completed += OnLoadedSceneFadeComplete;
        }
    }

    private void OnLoadedSceneFadeComplete(AsyncOperation obj)
    {
        if (fadeAnimator != null && fadeAnimator.isActiveAndEnabled)
        {
            fadeAnimator.SetBool("FadeIn", true);
            fadeAnimator.SetBool("FadeOut", false);
            fadeAnimator.SetBool("SlowFadeIn", false);
            fadeAnimator.SetBool("SlowFadeOut", false);
        }
        else
        {
            Debug.Log("fade in completed fail!");
        }
        isLoading = false;
    }

    private IEnumerator loadSceneSlowFadeIn(string sceneName)
    {
        if (isLoading)
            yield return null;
        else
        {
            isLoading = true;

            if (fadeAnimator != null && fadeAnimator.isActiveAndEnabled)
            {
                fadeAnimator.SetBool("FadeIn", false);
                fadeAnimator.SetBool("FadeOut", true);
                fadeAnimator.SetBool("SlowFadeIn", false);
                fadeAnimator.SetBool("SlowFadeOut", false);
            }
            else
            {
                Debug.Log("animator fail " + fadeAnimator);
            }
            yield return new WaitForSeconds(1);

            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            //async.completed += UnloadCurrentScene;
            async.completed += OnLoadedSceneSlowFadeInCompleted;
        }
    }

    private void OnLoadedSceneSlowFadeInCompleted(AsyncOperation obj)
    {
        if (fadeAnimator != null && fadeAnimator.isActiveAndEnabled)
        {
            fadeAnimator.SetBool("FadeIn", false);
            fadeAnimator.SetBool("FadeOut", false);
            fadeAnimator.SetBool("SlowFadeIn", true);
            fadeAnimator.SetBool("SlowFadeOut", false);
        }
        else
        {
            Debug.Log("slow fade in completed fail!");
        }
        isLoading = false;

        GameEvents.Instance.NewSceneLoaded();
    }

    //private void UnloadCurrentScene(AsyncOperation obj)
    //{
    //    if (!string.IsNullOrEmpty(currentSceneName))
    //    {
    //        SceneManager.UnloadSceneAsync(currentSceneName);
    //    }
    //    else
    //    {
    //        Debug.Log("unload currentScene fail!: " + currentSceneName);
    //        Debug.Log("nextScene: " + nextSceneName);
    //    }
    //    currentSceneName = nextSceneName;
    //    nextSceneName = null;
    //}

    private void ResetFadeAnimatorParameters()
    {
        fadeAnimator.SetBool("FadeIn", false);
        fadeAnimator.SetBool("FadeOut", false);
        fadeAnimator.SetBool("SlowFadeIn", false);
        fadeAnimator.SetBool("SlowFadeOut", false);
    }
}
