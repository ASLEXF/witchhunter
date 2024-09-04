using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.IO;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class TimeLineManager : MonoBehaviour
{
    private static TimeLineManager _instance;

    public static TimeLineManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("TimeLineManager");
                _instance = singletonObject.AddComponent<TimeLineManager>();
            }
            return _instance;
        }
    }

    PlayableDirector _playableDirector;
    string configFilePath = "Assets/Config/TimelineTrackBindings.json";
    Dictionary<string, GameObject> bindingTable = new Dictionary<string, GameObject>();

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

        _playableDirector = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        _playableDirector.playableAsset = null;

        GameEvents.Instance.OnDialogBoxStarted += PauseTimeline;
        GameEvents.Instance.OnDialogBoxEnded += PlayTimeLine;

        //_playableDirector.played += OnTimelinePlayed =>
        //{
        //    PlayerController.Instance.enabled = false;
        //};

        //_playableDirector.stopped += OnTimelineStopped =>
        //{
        //    PlayerController.Instance.enabled = true;
        //};

        Addressables.InitializeAsync().Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // Now you can safely use Addressables.LoadAssetAsync
                //LoadPlayableAsset();
            }
            else
            {
                Debug.LogError("Failed to initialize Addressables.");
            }
        };
    }

    private void Update()
    {
        // debug
        if (_playableDirector == null) {
            Debug.LogError("playable director not found!");
        }
    }

    public void PlayTimeLine()
    {
        if (_playableDirector != null && _playableDirector.playableAsset != null)
        {
            _playableDirector.Play();
        }
    }

    public void PauseTimeline()
    {
        if (_playableDirector != null && _playableDirector.playableAsset != null)
        {
            _playableDirector.Pause();
        }
    }

    public void StopTimeLine()
    {
        if (_playableDirector != null && _playableDirector.playableAsset != null)
        {
            _playableDirector.Stop();
            _playableDirector = null;
        }
    }

    public void JumptTo(float time)
    {
        if (_playableDirector != null && _playableDirector.playableAsset != null)
        {
            _playableDirector.time = time;
            _playableDirector.Play();
        }
    }

    public void WaitDialogBoxEnded()
    {
        GameEvents.Instance.OnDialogBoxEnded += () => PlayTimeLine();
    }

    public void OnTimelineJump(float time)
    {
        JumptTo(time);
    }

    public void LoadPlayableAsset(string fileName)
    {
        string file = "Assets/Addressables/Timelines/" + fileName + ".playable";
        Addressables.LoadAssetAsync<PlayableAsset>(file).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _playableDirector.playableAsset = handle.Result;
                GameEvents.Instance.PlayableAssetLoaded();
            }
            else
            {
                Debug.LogError("Addressables PlayableAsset not found: " + fileName);
            }
        };
    }

    public void LoadBindingTable(string timeLineFileName)
    {
        if (File.Exists(configFilePath))
        {
            string json = File.ReadAllText(configFilePath);
            var timelines = JsonConvert.DeserializeObject<Dictionary<string, List<BindingConfigEntry>>>(json);

            if (timelines == null)
            {
                Debug.LogError("Failed to deserialize JSON.");
                return;
            }

            if (timelines.TryGetValue(timeLineFileName, out List<BindingConfigEntry> configs))
            {
                bindingTable.Clear();
                foreach (var config in configs)
                {
                    bool hasFound = false;

                    Scene scene = SceneManager.GetSceneByName(config.sceneName);
                    if (!scene.isLoaded)
                    {
                        Debug.LogWarning($"Scene {config.sceneName} is not loaded");
                    }

                    foreach (GameObject rootObject in scene.GetRootGameObjects())
                    {
                        if (rootObject.name == config.gameObjectPath)
                        {
                            bindingTable.Add(config.trackName, rootObject);
                            hasFound = true;
                            break;
                        }
                        Transform childTransform = rootObject.transform.Find(config.gameObjectPath);
                        if (childTransform != null)
                        {
                            bindingTable.Add(config.trackName, childTransform.gameObject);
                            hasFound = true;
                            break;
                        }
                    }

                    if (!hasFound)
                    {
                        Debug.LogWarning($"{config.gameObjectPath} not found!");
                    }
                }
            }
            else
            {
                Debug.LogError("No configurations found for the specified timeline.");
            }
        }
        else
        {
            Debug.LogError($"Config file not found at {configFilePath}");
        }
        GameEvents.Instance.OnPlayableAssetLoaded += bindTimelineTracks;
    }

    //GameObject FindInChildren(Transform parent, string name)
    //{
    //    foreach (Transform child in parent)
    //    {
    //        if (child.name == name)
    //        {
    //            return child.gameObject;
    //        }
    //        GameObject found = FindInChildren(child, name);
    //        if (found != null)
    //        {
    //            return found;
    //        }
    //    }
    //    return null;
    //}

    void bindTimelineTracks()
    {
        PlayableAsset playableAsset = _playableDirector.playableAsset;
        if (playableAsset == null)
        {
            Debug.LogError("PlayableAsset is not assigned in the PlayableDirector.");
            return;
        }

        foreach (var binding in playableAsset.outputs)
        {
            string trackName = binding.streamName;

            if (bindingTable.ContainsKey(trackName))
            {
                _playableDirector.SetGenericBinding(binding.sourceObject, bindingTable[trackName]);
            }
        }

        PlayTimeLine();
    }
}
