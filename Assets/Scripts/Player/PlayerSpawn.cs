using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    GameObject spawnPoint;
    PlayerHealth playerHealth;

    private void Awake()
    {
        spawnPoint = GameObject.FindWithTag("SpawnPoint");
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Start()
    {
        if (spawnPoint != null)
        {
            PlayerController.Instance.transform.position = spawnPoint.transform.position;
        }
    }

    public void PlayerRespawn()
    {
        if (spawnPoint != null)
        {
            PlayerController.Instance.transform.position = spawnPoint.transform.position;
        }

        playerHealth.revive();
    }
}
