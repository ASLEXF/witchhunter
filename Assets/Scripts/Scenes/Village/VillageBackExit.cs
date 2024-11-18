using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VillageBackExit : Interactive
{
    public new bool isInteractable = true;

    private void Start()
    {
        
    }

    public override void Interacted()
    {
        base.Interacted();
        SceneLoader.Instance.LoadScene("ForestB1");
    }
}
