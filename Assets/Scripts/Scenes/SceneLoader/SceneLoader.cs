using System.Collections;
using System.Collections.Generic;
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

    public List<string> battleScenes = new List<string>() { "Cliff", "Forest" };
    public List<string> explorationScenes = new List<string>() { "Village", "Room", "Basement" };

    public Animator fadeAnimator;

    [SerializeField] private List<string> _loadedScenes = new List<string>();
    //[SerializeField] private string currentSceneName;
    //[SerializeField] private string nextSceneName;
    private bool isLoading = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void GameSceneInitiate()
    {
        UnloadCurrentScenes();
        LoadScene("Persistent");
        LoadScene("Main Menu");
    }

    void UnloadCurrentScenes()
    {
        int sceneCount = SceneManager.sceneCount;

        for (int i = sceneCount - 1; i >= 0; i--)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == "Persistent")
            {
                _loadedScenes.Add(scene.name);
                continue;
            }
            if (scene.isLoaded)
            {
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
                Debug.Log($"Unloaded scene: {scene.name}");
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        if (_loadedScenes.Contains(sceneName)) { return; }
        else if (_loadedScenes.Count > 1)
        {
            SceneManager.UnloadSceneAsync(_loadedScenes[1]);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            _loadedScenes.Remove(_loadedScenes[1]);
            _loadedScenes.Add(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            _loadedScenes.Add(sceneName);
        }
    }

    public void LoadSceneSlowFadeIn(string sceneName)
    {
        if(_loadedScenes.Contains(sceneName)) { return; }
        else if(_loadedScenes.Count > 1)
        {
            SceneManager.UnloadSceneAsync(_loadedScenes[1]);
            StartCoroutine(loadSceneSlowFadeIn(sceneName));
            _loadedScenes.Remove(_loadedScenes[1]);
            _loadedScenes.Add(sceneName);
        }
        else
        {
            StartCoroutine(loadSceneSlowFadeIn(sceneName));
            _loadedScenes.Add(sceneName);
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
            async.completed += OnLoadedSceneSlowFadeInComplete;
        }
    }

    private void OnLoadedSceneSlowFadeInComplete(AsyncOperation obj)
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
