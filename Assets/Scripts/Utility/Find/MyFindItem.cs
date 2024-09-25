using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyFindItem : MonoBehaviour
{
    private static MyFindItem instance;

    public static MyFindItem Instance
    { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }

    public GameObject FindGameObjectInScene(string name, string sceneName = null)
    {
        if (sceneName != null)
        {
            UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                Debug.LogWarning($"Scene {sceneName} is not loaded");
            }

            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                if (rootObject.name == name)
                {
                    return rootObject;
                }

                Transform childTransform = rootObject.transform.Find(name);
                if (childTransform != null)
                {
                    return childTransform.gameObject;
                }
            }
        }
        else
        {
            foreach(string _sceneName in SceneLoader.Instance.CurrentScenes)
            {
                UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneByName(_sceneName);
                if (!scene.isLoaded)
                {
                    Debug.LogWarning($"Scene {sceneName} is not loaded");
                }

                foreach (GameObject rootObject in scene.GetRootGameObjects())
                {
                    if (rootObject.name == name)
                    {
                        return rootObject;
                    }

                    Transform childTransform = rootObject.transform.Find(name);
                    if (childTransform != null)
                    {
                        return childTransform.gameObject;
                    }
                }
            }
        }

        Debug.LogWarning($"game object {name} not found!");
        return null;
    }
}
