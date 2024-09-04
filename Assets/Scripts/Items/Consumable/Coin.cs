using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public MaterialItem item;

    private void Start()
    {
        item = new MaterialItem(0, name, "Money", "", 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
