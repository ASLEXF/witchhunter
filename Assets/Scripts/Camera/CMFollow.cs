using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CMFollow : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cm;
    [SerializeField] CinemachineVirtualCamera cm2;

    CinemachineConfiner2D confiner;

    private void Awake()
    {
        cm = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void Start()
    {
        GameEvents.Instance.OnNewSceneLoaded += LoadConfiner;
    }

    private void Update()
    {
        cm.transform.rotation = new Quaternion();
    }

    void SwitchToCM2()
    {

    }

    public void LoadConfiner()
    {
        Scene scene = SceneManager.GetSceneByName(SceneLoader.Instance.CurrentScenes[SceneLoader.Instance.CurrentScenes.Count - 1]);
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            if (rootObject.name == "CameraConfiner")
            {
                confiner.m_BoundingShape2D = rootObject.GetComponent<PolygonCollider2D>();
                break;
            }
        }

        if (confiner == null)
        {
            Debug.LogWarning("CameraConfiner not found!");
        }
    }
}
