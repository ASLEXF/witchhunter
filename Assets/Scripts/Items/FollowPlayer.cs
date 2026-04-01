using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Vector3 offset;

    private void FixedUpdate()
    {
        if (PlayerController.Instance != null)
        {
            transform.position = PlayerController.Instance.transform.position + offset;
        }
    }
}
