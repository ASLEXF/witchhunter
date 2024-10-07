using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MainCamera : MonoBehaviour
{
    CinemachineBrain brain;

    private void Awake()
    {
        brain = GetComponent<CinemachineBrain>();
    }
}
