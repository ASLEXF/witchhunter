using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] float factor;
    GameObject CMFollow;  // player follower camera
    Vector3 originalPosition, cameraOriginalPosition;

    private void Awake()
    {
        CMFollow = SceneManager.GetSceneByName("Persistent").GetRootGameObjects()[0].transform.Find("CM Follow").gameObject;
    }

    private void Start()
    {
        originalPosition = transform.position;
        cameraOriginalPosition = CMFollow.transform.position;
    }


    void Update()
    {
        transform.position = originalPosition + (CMFollow.transform.position - cameraOriginalPosition) * factor;
    }
}
