using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMTransition : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void MoveToPlayerFormerPosition()
    {
        transform.position = PlayerController.Instance.FormerPosition;
        transform.position += new Vector3(0, 0, -10);
    }
}
